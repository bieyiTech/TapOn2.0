using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;
using TapOn.Constants;

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
            createEdit.SetMapSession(mapSession);
            Prefabs.instance.arDisplay.SetActive(true);
            ShowParticle(true);
        }

        public void SecondEditMap()
        {
            CreateSession();

        }

        public void PreviewMap(List<String> mapIDs)
        {
            // 根据IDs到bmob数据库查找对应metas
            // foreach(meta in metas)
            // selectedMaps.Add(meta);
            CreateSession();
            mapSession.LoadMapMeta(MapControllerPrefab, false);
            

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
