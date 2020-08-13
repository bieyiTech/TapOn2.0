using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;
using TapOn.Constants;
using TapOn.Models.DataModels;
using TapOn.Api;

namespace AREffect
{
    public class AREffectManager : MonoBehaviour
    {
        public static AREffectManager Instance;
        public GameObject EasyARSession;
        public SparseSpatialMapController MapControllerPrefab;
        public Text Status;
        public Toggle PointCloudUI;
        public CreateEditMapController createEdit;

        private GameObject easyarObject;
        private ARSession session;
        private VIOCameraDeviceUnion vioCamera;
        private SparseSpatialMapWorkerFrameFilter mapFrameFilter;
        private List<MapMeta> selectedMaps = new List<MapMeta>();
        private MapSession mapSession;

        private void Awake()
        {
            Instance = this;
        }

        // Update is called once per frame
        private void Update()
        {
            if (session)
            {
                Status.text = "VIO Device" + Environment.NewLine +
                    "\tType: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                    "\tTracking Status: " + (session.WorldRootController == null ? "-" : session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                    "Sparse Spatial Map" + Environment.NewLine +
                    "\tWorking Mode: " + mapFrameFilter.WorkingMode + Environment.NewLine +
                    "\tLocalization Mode: " + mapFrameFilter.LocalizerConfig.LocalizationMode + Environment.NewLine +
                    "Localized Map" + Environment.NewLine +
                    "\tName: " + (mapFrameFilter.LocalizedMap == null ? "-" : (mapFrameFilter.LocalizedMap.MapInfo == null ? "-" : mapFrameFilter.LocalizedMap.MapInfo.Name)) + Environment.NewLine +
                    "\tID" + (mapFrameFilter.LocalizedMap == null ? "-" : (mapFrameFilter.LocalizedMap.MapInfo == null ? "-" : mapFrameFilter.LocalizedMap.MapInfo.ID)) + Environment.NewLine +
                    "\tPoint Cloud Count: " + (mapFrameFilter.LocalizedMap == null ? "-" : mapFrameFilter.LocalizedMap.PointCloud.Count.ToString());
                if (mapFrameFilter.LocalizedMap == null)
                {
                    PointCloudUI.gameObject.SetActive(false);
                }
                else
                {
                    PointCloudUI.gameObject.SetActive(true);
                }
            }
            else
            {
                Status.text = string.Empty;
            }
        }
        private void OnDestroy()
        {
            DestroySession();
        }
        
        public void CreateAndEditMap()
        {
            CreateSession();
            mapSession.SetupMapBuilder(MapControllerPrefab);
            createEdit.gameObject.SetActive(true);
            if(mapSession == null)
            {
                Debug.Log("AREffectManager: mapSession is null");
            }
            createEdit.SetMapSession(mapSession);
            Globals.instance.arDisplay.SetActive(true);
            ShowParticle(true);
        }

        public void CreateAndEditMapEnd()
        {
            createEdit.gameObject.SetActive(false);
            DestroySession();
            Globals.instance.arDisplay.SetActive(false);
            ShowParticle(false);
        }

        public void SecondEditMap()
        {
            CreateSession();

        }

        public IEnumerator PreviewMap(Mark mark)
        {
            //List<Prop> props = await BmobApi.getPropsInMark(mark);
            //List<MapMeta.PropInfo> propinfo = new List<MapMeta.PropInfo>();
            //foreach(Prop prop in props)
            //{
            //    propinfo.Add(
            //        new MapMeta.PropInfo
            //        {
            //            Position = new float[3] { (float)prop.pos_x.Get(), (float)prop.pos_y.Get(), (float)prop.pos_z.Get() },
            //            Rotation = new float[4] { (float)prop.rot_x.Get(), (float)prop.rot_y.Get(), (float)prop.rot_z.Get(), (float)prop.rot_w.Get() },
            //            Scale = new float[3] { (float)prop.scale_x.Get(), (float)prop.scale_y.Get(), (float)prop.scale_z.Get() },

            //        });
            //}
            //selectedMaps.Add(new MapMeta(new SparseSpatialMapController.SparseSpatialMapInfo { ID = mark.MapId, Name = mark.MapName }, propinfo));
            // Load Meta 文件即可
            // 如果本地有文件，获取之
            Debug.Log("PreviewMap");
            var cor = StartCoroutine("LoadMetaFile", mark);
            CreateSession();
            yield return cor;
            mapSession.LoadMapMeta(MapControllerPrefab, false);
            ShowParticle(false);
        }

        public IEnumerable LoadMetaFile(Mark mark)
        {
            if (MapMetaManager.isLocal(mark.MapId))
            {
                Debug.Log("is Local");
                selectedMaps.Add(MapMetaManager.LoadMeta(mark.MapId));
            }       // 否则，从url获取, 并保存在本地
            else
            {
                Debug.Log("get for url");
#pragma warning disable CS0618 // 类型或成员已过时
                WWW meta = new WWW(mark.metaFile.url);
#pragma warning restore CS0618 // 类型或成员已过时
                yield return meta;
                selectedMaps.Add(JsonUtility.FromJson<MapMeta>(meta.text));
                // 保存在本地
                foreach (var m in selectedMaps)
                {
                    if (!MapMetaManager.isLocal(m.Map.ID))
                        MapMetaManager.Save(m);
                }
            }
        }

        public void ShowParticle(bool show)
        {
            if (mapSession == null)
            {
                return;
            }
            foreach (var map in mapSession.Maps)
            {
                if (map.Controller)
                {
                    map.Controller.ShowPointCloud = show;
                }
            }
        }

        private void CreateSession()
        {
            easyarObject = Instantiate(EasyARSession, Instance.transform);
            easyarObject.SetActive(true);
            session = easyarObject.GetComponent<ARSession>();
            vioCamera = easyarObject.GetComponentInChildren<VIOCameraDeviceUnion>();
            mapFrameFilter = easyarObject.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();
            mapSession = new MapSession(mapFrameFilter, selectedMaps);
        }

        private void DestroySession()
        {
            if (mapSession != null)
            {
                mapSession.Dispose();
                mapSession = null;
            }
            if (easyarObject)
            {
                Destroy(easyarObject);
            }
        }
    }
}
