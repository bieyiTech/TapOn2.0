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

    public abstract class BaseProp
    {
        public ProductType type;
        public Vector3 position;
        public Vector4 rotation;
        public Vector3 scale;
        public GameObject instance;

        public abstract void instantiate();
    }

    public class TextProp : BaseProp
    {
        public string text;

        public override void instantiate()
        {
            throw new System.NotImplementedException();
        }
    }

    public class ImageProp : BaseProp
    {
        public string fileName;
        public string url;

        public override void instantiate()
        {
            throw new System.NotImplementedException();
        }
    }

    public class VideoProp : BaseProp
    {
        public string fileName;
        public string url;

        public override void instantiate()
        {
            throw new System.NotImplementedException();
        }
    }

    public class ModelProp : BaseProp
    {
        public string fileName;
        public string url;

        public override void instantiate()
        {
            throw new System.NotImplementedException();
        }
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
        public bool Downloading;
        public bool isLocal;
        public float progress;
        public string previewFileName;
        public string previewUrl;
        public string modelFileName;
        public string modelUrl;
        public string assetName;
        public string assetUrl;
    }
}
