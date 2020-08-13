using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AREffect
{
    public class MapMetaManager
    {
        private static readonly string root = Application.persistentDataPath + "/TapOn";

        public static List<MapMeta> LoadAll()
        {
            var metas = new List<MapMeta>();
            var dirRoot = GetRootPath();
            try
            {
                foreach(var path in Directory.GetFiles(dirRoot, "*.json"))
                {
                    try
                    {
                        metas.Add(JsonUtility.FromJson<MapMeta>(File.ReadAllText(path)));
                    }catch(System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            return metas;
        }
        
        public static MapMeta LoadMeta(string id)
        {
            return JsonUtility.FromJson<MapMeta>(File.ReadAllText(GetPath(id)));
        }

        public static bool isLocal(string id)
        {
            if(!File.Exists(GetPath(id)))
            {
                return false;
            }
            return true;
        }

        public static void SaveTextureToLocal(Texture2D tex, string fileName)
        {
            if(isTextureInLocal(fileName))
            {
                return;
            }
            var bytes = tex.EncodeToJPG();
            var path = PathForFile(fileName, "picture");
            //Debug.Log("Local Path: " + path);
            File.WriteAllBytes(path, bytes);
        }

        public static bool isTextureInLocal(string fileName)
        {
            var path = PathForFile(fileName, "picture");
            if(!File.Exists(path))
            {
                return false;
            }
            return true;
        }
        
        public static bool Save(MapMeta meta)
        {
            try
            {
                File.WriteAllText(GetPath(meta.Map.ID), JsonUtility.ToJson(meta));
            }catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }

        public static bool Delete(MapMeta meta)
        {
            if(!File.Exists(GetPath(meta.Map.ID)))
            {
                return false;
            }
            try
            {
                File.Delete(GetPath(meta.Map.ID));
            }catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }

        private static string GetRootPath()
        {
            var path = root;
            if(!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetPath(string id)
        {
            return GetRootPath() + "/" + id + ".json";
        }

        /// <summary>
        /// 在不同平台保存
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string PathForFile(string filename, string dic)
        {

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                string path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.Length - 5);
                path = path.Substring(0, path.LastIndexOf('/'));
                path = Path.Combine(path, "Documents");
                path = Path.Combine(path, dic);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return Path.Combine(path, filename);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                string path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                path = Path.Combine(path, dic);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return Path.Combine(path, filename);
            }
            else
            {
                string path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                path = Path.Combine(path, dic);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return Path.Combine(path, filename);
            }
        }
    }
}
