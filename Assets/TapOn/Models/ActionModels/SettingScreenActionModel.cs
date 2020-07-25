using System;
using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using UnityEngine;

namespace TapOn.Models.ActionModels
{
    public class SettingScreenActionModel
    {
        public Action<int> ChangeIndex;

        public Action<string> AddTextProduct;
        public Action<Texture2D> AddImageProduct;
        public Action<string> AddVideoProduct;

        public Action<List<Model>> SetModelsMessage;
    }
}
