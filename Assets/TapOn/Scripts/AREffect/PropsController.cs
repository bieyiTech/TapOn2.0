using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TapOn.Models.DataModels;

namespace AREffect
{
    public class PropsController : MonoBehaviour
    {
        public GameObject OutlinePrefab;
        public GameObject FreeMove;
        public UnityEngine.UI.Toggle VideoPlayable;
        
        private MapSession mapSession;
        private GameObject candidate;
        private GameObject selection;
        private TouchController touchControl;
        private bool isOnMap;
        private bool isMoveFree = true;
        private int tempCount = 0;

        public event Action<GameObject> CreateObject;
        public event Action<GameObject> DeleteObject;
        
        private void Awake()
        {
            touchControl = GetComponentInChildren<TouchController>(true);
            OutlinePrefab = Instantiate(OutlinePrefab);
            OutlinePrefab.SetActive(false);
        }
        
        private void Update()
        {
            var isEditorOrStandalone = Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer;
            var isPointerOverGameObject = (isEditorOrStandalone && EventSystem.current.IsPointerOverGameObject())
                || (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
            
            if (candidate)
            {
                
                //if (mapSession != null && !isPointerOverGameObject && Input.touchCount > 0)
                if (mapSession != null && Input.touchCount > 0)
                {
                    var point = mapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width, Input.touches[0].position.y / Screen.height));
                    //Debug.Log(point);
                    if (point.OnSome)
                    {
                        //Debug.Log("point.OnSome");
                        candidate.transform.position = point.Value + Vector3.up * candidate.transform.localScale.y / 2;
                        isOnMap = true;
                    }
                }

                if (!isOnMap)
                {
                    //Debug.Log("candidate is false");
                    candidate.SetActive(false);
                }
                else
                {
                    //Debug.Log("candidate is true");
                    candidate.SetActive(true);
                }

            }
            else
            {
                //if (!isPointerOverGameObject && ((isEditorOrStandalone && Input.GetMouseButtonDown(0)) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)))
                if (((isEditorOrStandalone && Input.GetMouseButtonDown(0)) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        //Debug.Log("hitInfo: " + hitInfo);
                        StopEdit();
                        StartEdit(hitInfo.collider.gameObject);
                    }
                }
            }

            if (mapSession != null && selection && !isMoveFree)
            {
                //Debug.Log("selection");
                if (!isPointerOverGameObject && Input.touchCount == 1)
                {
                    var point = mapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width, Input.touches[0].position.y / Screen.height));
                    if (point.OnSome)
                    {
                        //Debug.Log("point on some");
                        selection.transform.position = point.Value + Vector3.up * selection.transform.localScale.y / 2;
                    }
                }
            }
        }

        private void OnDisable()
        {
            mapSession = null;
            StopEdit();
        }

        public void SetMapSession(MapSession session)
        {
            mapSession = session;
            
            if (mapSession.MapWorker)
            {
                mapSession.MapWorker.MapLoad += (arg1, arg2, arg3, arg4) =>
                {
                    StartCoroutine(CheckVideo());
                };
            }
        }

        public void SetFreeMove(bool free)
        {
            isMoveFree = free;
            if (selection)
            {
                if (free)
                {
                    touchControl.TurnOn(selection.transform, Camera.main, true, true, true, true);
                }
                else
                {
                    touchControl.TurnOn(selection.transform, Camera.main, false, false, true, true);
                }
            }
        }

        public void StartCreate(Prop prop)
        {
            StopEdit();
            isOnMap = false;
            Debug.Log("start instance: " +prop.instance.tag);
            candidate = Instantiate(prop.instance);
            Debug.Log("start: " + candidate.tag);
            if (candidate)
            {
                var video = candidate.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = false; }
            }
            FreeMove.SetActive(false);
            candidate.SetActive(false);
        }

        public void StopCreate()
        {
            if (candidate.activeSelf)
            {
                if (CreateObject != null)
                {
                    Debug.Log("stop: " + candidate.tag);
                    CreateObject(candidate);
                    StartEdit(candidate);
                }
            }
            else
            {
                Destroy(candidate);
            }
            FreeMove.SetActive(true);
            isOnMap = false;
            candidate = null;
        }

        public void StartEdit(GameObject obj)
        {
            selection = obj;
            if (selection && VideoPlayable.isOn)
            {
                var video = selection.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = true; }
            }
            if(selection.tag == "texture")
            {
                var meshFilter = selection.GetComponentInChildren<MeshFilter>();
                OutlinePrefab.SetActive(true);
                OutlinePrefab.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
                OutlinePrefab.transform.parent = meshFilter.transform;
                
            }
            else if(selection.name == "word")
            {
                var TextMesh = selection.GetComponentInChildren<TextMesh>();
                OutlinePrefab.SetActive(false);
                OutlinePrefab.transform.parent = TextMesh.transform;
            }
            
            OutlinePrefab.transform.localPosition = Vector3.zero;
            OutlinePrefab.transform.localRotation = Quaternion.identity;
            OutlinePrefab.transform.localScale = Vector3.one;
            
            SetFreeMove(isMoveFree);
        }

        public void StopEdit()
        {
            if (selection)
            {
                var video = selection.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = false; }
            }
            selection = null;
            if (OutlinePrefab)
            {
                OutlinePrefab.transform.parent = null;
                OutlinePrefab.SetActive(false);
            }
            if (touchControl)
            {
                touchControl.TurnOff();
            }
        }

        public void DeleteSelection()
        {
            if (!selection)
            {
                return;
            }
            if (DeleteObject != null)
            {
                DeleteObject(selection);
            }
            Destroy(selection);
            StopEdit();
        }

        public void ToggleVideoPlayable(bool playable)
        {
            if (selection)
            {
                var video = selection.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = playable; }
            }
        }

        private IEnumerator CheckVideo()
        {
            yield return new WaitForEndOfFrame();
            if (mapSession == null) { yield return 0; }
            foreach (var prop in mapSession.Maps[0].Props)
            {
                if (prop)
                {
                    var video = prop.GetComponentInChildren<VideoPlayerAgent>(true);
                    if (video) { video.Playable = false; }
                }
            }
        }
    }
}