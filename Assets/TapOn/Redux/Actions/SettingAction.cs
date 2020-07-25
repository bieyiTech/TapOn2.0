using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using UnityEngine;

namespace TapOn.Redux.Actions
{
    public class ChangeIndexAction
    {
        public int index;
    }

    public class AddProductAction
    {
        public Product product;
    }

    public class AddTextProductAction
    {
        public string text;
    }

    public class AddImageProductAction
    {
        public Texture2D texture;
    }

    public class AddVideoProductAction
    {
        public string path;
    }

    public class SetModelsMessageAction
    {
        public List<Model> models;
    }
}
