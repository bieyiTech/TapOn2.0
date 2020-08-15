using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;
using TapOn.Constants;
using TapOn.Models.DataModels;
using TapOn.Api;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.async;

namespace AREffect
{
    public class AREffectManager : MonoBehaviour
    {
        public static AREffectManager Instance;
        public GameObject EasyARSession;
        public SparseSpatialMapController MapControllerPrefab;
        public Text Status;
        //public Toggle PointCloudUI;
        public CreateEditMapController createEdit;
        public PreviewEditController previewEdit;

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
                    //PointCloudUI.gameObject.SetActive(false);
                }
                else
                {
                    //PointCloudUI.gameObject.SetActive(true);
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
            Globals.instance.arEffect.SetActive(false);
            ShowParticle(false);
        }
        
        public IEnumerator PreviewMap(Mark mark)
        {
            Debug.Log("PreviewMap");
            previewEdit.gameObject.SetActive(true);
            yield return Window.instance.startCoroutine(LoadMetaFile(mark));
            CreateSession();
            previewEdit.SetMapSession(mapSession);
            mapSession.LoadMapMeta(MapControllerPrefab, false);
            ShowParticle(false);
        }

        public void PreviewMapEnd()
        {
            previewEdit.gameObject.SetActive(false);
            DestroySession();
        }

        private IEnumerator LoadMetaFile(Mark mark)
        {
            Debug.Log("LoadMetaFile");
            
            if (MapMetaManager.isLocal(mark.MapId))
            {
                Debug.Log("is Local");
                selectedMaps.Add(MapMetaManager.LoadMeta(mark.MapId));
            }       // 否则，从url获取, 并保存在本地
            else
            {
                Debug.Log("get from url");
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
