using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;

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
        
        public void Snapshot()
        {
            var oneShot = Camera.main.gameObject.AddComponent<OneShot>();
            oneShot.Shot(true, (texture) =>
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