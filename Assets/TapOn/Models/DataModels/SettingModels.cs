using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Models.DataModels
{
    public enum ProductType
    {
        Text,
        Picture,
        Video,
        Model
    }
    /// <summary>
    /// 放置界面的半成品（选好图的图片）
    /// </summary>
    public class Product
    {
        public ProductType type;
        public GameObject instance;
    }

    public enum ModelType
    {
        All,
        Scene,
        People
    }

    public class Model
    {
        public static List<string> typeNames = new List<string> { "全部", "场景", "人物" };

        public int modelType;
        public string id;
        public string modelName;
        public string previewFileName;
        public string previewUrl;
        public string modelFileName;
        public string modelUrl;
    }
}
