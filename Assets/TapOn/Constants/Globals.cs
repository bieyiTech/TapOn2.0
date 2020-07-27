using cn.bmob.api;
using System.Collections;
using System.Collections.Generic;
using TencentMap.API;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Constants {
    public class Globals : MonoBehaviour
    {
        private static Globals _instance;
        public static Globals instance
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

        public List<GameObject> models;
        public GameObject marker;
        public GameObject map;

        public Stack<BuildContext> contextStack = new Stack<BuildContext>();
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
