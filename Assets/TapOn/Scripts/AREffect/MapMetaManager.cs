﻿using System.Collections.Generic;
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
    }
}
