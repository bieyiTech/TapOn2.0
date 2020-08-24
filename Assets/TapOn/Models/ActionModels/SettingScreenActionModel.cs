using System;
using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using UnityEngine;

namespace TapOn.Models.ActionModels
{
    public class SettingScreenActionModel
    {
        

        public Action<string> AddTextProduct;
        public Action<Texture2D> AddImageProduct;
        public Action<string> AddVideoProduct;

        /// <summary>
        /// 从服务器拉取所有的模型信息
        /// </summary>
        public Action<List<Model>> SetModelsMessage;
        public Action<int, bool> ChangeModelLocalStateByIndex;
        public Action<int, bool> ChangeModelDownloadingStateByIndex;
        public Action<int, float> ChangeModelProgressByIndex;

        public Action<bool> ChangeSpanState;
        public Action<bool> ChangeShowState;
        public Action<bool, int> ChangeAppearState;
        public Action ChangeProductIndex;
        public Action<bool> ChangeMovebycircleState;
        public Action<int> ChangeModelIndex;
        public Action<bool> ChangeModelMessageReadyState;
        public Action<int> ChangeCameraType;

        public Func<string, object> AddTextProductFuc;
        public Func<Texture2D, Unity.UIWidgets.widgets.BuildContext, object> AddImageProductFuc;
        public Func<GameObject, string, object> AddModelProductFuc;

    }
}
