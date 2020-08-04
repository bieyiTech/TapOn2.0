using cn.bmob.api;
using System.Collections;
using System.Collections.Generic;
using TencentMap.API;
using Unity.UIWidgets.widgets;
using UnityEngine;
using AREffect;

namespace TapOn.Constants {
    public class Globals : MonoBehaviour
    {
        private static Globals _instance;
        private static GameObject go;
        public static Globals instance
        {
            get
            {
                if (_instance == null)  // 如果没有找到
                {
                    GameObject t = GameObject.Find("global");
                    if (t == null)
                    {
                        go = new GameObject("_globals"); // 创建一个新的GameObject
                                                         //DontDestroyOnLoad(go);  // 防止被销毁
                        _instance = go.AddComponent<Globals>(); // 将实例挂载到GameObject上
                    }
                    else if (t.GetComponent<Globals>() == null)
                    {
                        _instance = t.AddComponent<Globals>();
                    }
                    else
                        _instance = t.GetComponent<Globals>();
                }
                return _instance;
            }
        }

        public void Awake()
        {
            GameObject t = GameObject.Find("global");
            if(t == null)
            {
                Debug.LogError("Can not Found global");
            }
            else
            {
                _instance = t.GetComponent<Globals>();
            }
        }

        public List<GameObject> models;
        public GameObject marker;
        public GameObject map;

        public Stack<BuildContext> contextStack = new Stack<BuildContext>();
        public BuildContext homeContext;

        public BmobUnity bmob;
        public MapController mapController;

        public MarkManager markManager;

        public GameObject arEffect;
        public GameObject arDisplay;
        
        public List<GameObject> templetes;
        public PropsController dragger;

        public CreateEditMapController CreateEdit;

        public Camera mapCamera;

        // Start is called before the first frame update

        public void CheckInstance()
        {
            if (instance.bmob == null)
            {
                GameObject t = GameObject.Find("Config");
                if (t == null) Debug.LogError("Config in scene not found!");
                else
                {
                    Globals.instance.bmob = t.GetComponent<BmobUnity>();
                }
            }
            if (mapController == null)
            {
                Debug.Log("mapcontriller null");
                GameObject t = GameObject.Find("map_mapObj_prefab");
                if (t == null) Debug.LogError("map_mapObj_prefab in scene not found!");
                else
                {
                    mapController = t.GetComponent<MapController>();
                }
                if (mapController == null) Debug.LogError("even nuill");
            }
            /*if(instance.arEffect == null)
            {
                GameObject t = GameObject.Find("Config");
                if (t == null) Debug.LogError("Config in scene not found!");
                else
                {
                    Globals.instance.bmob = t.GetComponent<BmobUnity>();
                }
            }*/
        }
        

    }
}
