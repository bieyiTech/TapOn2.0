using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.UIWidgets.ui;
using easyar;
using System;
using TapOn.Models.DataModels;
using TapOn.Api;
using System.IO;
using TapOn.Utils;
using cn.bmob.io;

namespace AREffect
{
    public class CreateEditMapController : MonoBehaviour
    {
        public PropsController PropDragger;
        public Button SaveButton;
        public RawImage PreviewImage;
        public SparseSpatialMapController MapControllerPrefab;
        public GameObject UploadPopup;

        private MapSession mapSession;
        private MapSession.MapData mapData;
        private bool withPreview = true;
        private string mapName;
        private Texture2D capturedImage;
        private int uploadingTime;
        private int tempCount = 0;
        private int tempInfoCount = 0;

        private void Awake()
        {
            PropDragger.CreateObject += (gameObj) =>
            {
                if (gameObj)
                {
                    gameObj.transform.parent = mapData.Controller.transform;
                    mapData.Props.Add(gameObj);
                }
            };
            PropDragger.DeleteObject += (gameObj) =>
            {
                if (gameObj)
                {
                    mapData.Props.Remove(gameObj);
                }
            };
        }

        private void OnEnable()
        {
            SaveButton.gameObject.SetActive(true);
            SaveButton.interactable = false;
            UploadPopup.SetActive(false);
        }

        private void Update()
        {
            if ((mapSession.MapWorker.LocalizedMap == null || mapSession.MapWorker.LocalizedMap.PointCloud.Count <= 20) && !Application.isEditor
                || mapSession.IsSaving || mapSession.Saved)
            {
                SaveButton.interactable = false;
            }
            else
            {
                if (tempCount++ >= 60)
                {
                    tempCount = 0;
                    //Debug.Log("Update: " + mapSession.Maps.Count);
                }

                SaveButton.interactable = true;
            }
        }

        private void OnDestroy()
        {
            if (capturedImage)
            {
                Destroy(capturedImage);
            }
        }

        public void SetMapSession(MapSession session)
        {
            mapSession = session;
            PropDragger.SetMapSession(session);
        }

        public void Upload()
        {
            Snapshot();
            mapSession.MapWorker.enabled = false;
            mapName = "Map_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss");

            mapSession.Save(mapName, null);
            StartCoroutine(SavingStatus());
            StartCoroutine(Saving());
        }

        /// <summary>
        /// 保存创作
        /// </summary>
        public void SaveEdit()
        {
            if (mapData == null)
            {
                return;
            }
            SaveMapMeta();
            //Snapshot();
            // 保存到云端
            // (图片)capturedImage
            // (ID)mapData.Meta.Map.ID
            // (time) DateTime.Now.ToString("yyyy-MM-dd_HHmmss")
            // (Meta) mapData.Meta
            // GPS
            Mark mark = new Mark
            {
                coordinate = new cn.bmob.io.BmobGeoPoint(39.986, 116.314),
                snapShot_byte = capturedImage.EncodeToJPG(),
                MapId = mapData.Meta.Map.ID,
                MapName = "Map_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss"),
                meta_byte = File.ReadAllBytes(MapMetaManager.GetPath(mapData.Meta.Map.ID)),
            };
            BmobApi.addMarktoServer(mark);
        }
        
        public void SaveMapMeta()
        {
            if (mapData == null)
            {
                return;
            }

            Debug.Log("length: " + mapData.Props.Count);

            var propInfos = new List<MapMeta.PropInfo>();

            foreach (var prop in mapData.Props)
            {
                var position = prop.transform.localPosition;
                var rotation = prop.transform.localRotation;
                var scale = prop.transform.localScale;
                MapMeta.PropType typeTemp;
                string textTemp = null;
                BmobFile infoTemp = null;

                Debug.Log("prop tag" + prop.tag);
                Debug.Log("prop name" + prop.name);

                if ("word" == prop.tag)
                {
                    typeTemp = MapMeta.PropType.Text;
                    textTemp = prop.GetComponentInChildren<TextMesh>().text;
                    propInfos.Add(new MapMeta.PropInfo()
                    {
                        Name = prop.name,
                        Position = new float[3] { position.x, position.y, position.z },
                        Rotation = new float[4] { rotation.x, rotation.y, rotation.z, rotation.w },
                        Scale = new float[3] { scale.x, scale.y, scale.z },
                        type = typeTemp,
                        text = textTemp,
                    });
                }
                else
                {
                    byte[] info_byte = null;
                    if ("texture" == prop.tag)
                    {
                        typeTemp = MapMeta.PropType.Texture;
                        Texture rt = prop.GetComponentInChildren<MeshRenderer>().material.mainTexture;
                        if (rt == null)
                            Debug.LogError("RenderTexture is error");
                        else
                        {
                            RenderTexture destTexture = new RenderTexture(rt.width, rt.height, 0);
                            Graphics.Blit(rt, destTexture);
                            RenderTexture.active = destTexture;
                            var imgTemp = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
                            imgTemp.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
                            imgTemp.Apply();
                            RenderTexture.active = null;
                            Destroy(destTexture);

                            info_byte = imgTemp.EncodeToJPG();
                            if (info_byte == null)
                                Debug.Log("info_byte is null");
                        }
                        //info_byte = prop.GetComponentInChildren<MeshRenderer>().material.mainTexture.
                        StartCoroutine(
                            TapOnUtils.upLoadFile(
                                "NameCard_" + (tempInfoCount++) + "_"+ DateTime.Now.ToString() + ".jpg",
                                "application/x-jpg",
                                info_byte,
                                (wr) =>
                                {
                                    Restful_FileUpLoadCallBack t = TapOnUtils.fileUpLoadCallBackfromJson(wr.downloadHandler.text);
                                    infoTemp = new BmobFile { filename = t.filename, url = t.url };
                                    Debug.Log("NameCard save");
                                    propInfos.Add(new MapMeta.PropInfo()
                                    {
                                        Name = prop.name,
                                        Position = new float[3] { position.x, position.y, position.z },
                                        Rotation = new float[4] { rotation.x, rotation.y, rotation.z, rotation.w },
                                        Scale = new float[3] { scale.x, scale.y, scale.z },
                                        type = typeTemp,
                                        infoFileName = infoTemp.filename,
                                        infoUrl = infoTemp.url,
                                    });
                                })
                            );
                    }
                    else if ("Video(Clone)" == prop.name)
                    {
                        typeTemp = MapMeta.PropType.Video;
                        // info_byte
                    }
                    else if ("Model(Clone)" == prop.name)
                    {
                        typeTemp = MapMeta.PropType.Model;
                        // info_byte
                    }
                    else
                    {
                        typeTemp = MapMeta.PropType.other;
                    }
                }
            }
            mapData.Meta.Props = propInfos;
            // 保存到本地
            MapMetaManager.Save(mapData.Meta);

        }

        public void Snapshot()
        {
            var oneShot = Camera.main.gameObject.AddComponent<OneShot>();
            oneShot.Shot(false, (texture) =>
            {
                if (capturedImage)
                {
                    Destroy(capturedImage);
                }
                capturedImage = texture;
                PreviewImage.texture = capturedImage;
            });
        }

        private IEnumerator Saving()
        {
            while(mapSession.IsSaving)
            {
                yield return 0;
            }
            if(mapSession.Saved)
            {
                mapSession.StopSetupMapBuilder();
                mapSession.MapWorker.enabled = true;
                mapSession.LoadMapMeta(MapControllerPrefab, true);
                mapData = mapSession.Maps[0];
            }
            else
            {
                var buttonText = SaveButton.GetComponentInChildren<Text>();
                buttonText.text = "Retry";
            }
        }

        private IEnumerator SavingStatus()
        {
            var buttonText = SaveButton.GetComponentInChildren<Text>();
            while(mapSession.IsSaving)
            {
                buttonText.text = "Saving";
                for(int i = 0; i < uploadingTime; i++)
                {
                    buttonText.text += ".";
                }
                uploadingTime = (uploadingTime + 1) % 3;
                yield return new WaitForSeconds(1);
            }
        }
    }
}