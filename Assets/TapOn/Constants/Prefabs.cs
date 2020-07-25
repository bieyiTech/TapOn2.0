using cn.bmob.api;
using System.Collections;
using System.Collections.Generic;
using TencentMap.API;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Constants {
    public class Prefabs : MonoBehaviour
    {
        private static Prefabs _instance;
        public static Prefabs instance
        {
            get
            {
                if (_instance == null)  // 如果没有找到
                {
                    GameObject t = GameObject.FindGameObjectWithTag("global");
                    if (t == null)
                    {
                        GameObject go = new GameObject("_globals"); // 创建一个新的GameObject
                                                                    //DontDestroyOnLoad(go);  // 防止被销毁
                        _instance = go.AddComponent<Prefabs>(); // 将实例挂载到GameObject上
                    }
                    else if (t.GetComponent<Prefabs>() == null)
                    {
                        _instance = t.AddComponent<Prefabs>();
                    }
                    else
                        _instance = t.GetComponent<Prefabs>();
                }
                return _instance;
            }
        }

        public List<GameObject> models;
        public GameObject marker;
        public GameObject map;
        public BuildContext homeContext;

        public BmobUnity bmob;
        public MapController mapController;

        public MarkManager markManager;
        public SpriteMask spriteMask;

        public List<GameObject> templetes;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
