using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Models.States
{
    public class SettingState
    {
        /// <summary>
        /// 文字 图片 视频 模型 图标映射
        /// </summary>
        public List<IconData> allIcons;
        /// <summary>
        /// 完成品队列，长度为3
        /// </summary>
        public Queue<Product> products;

        public List<Model> models;

        public List<GameObject> objects;
        public int index;
    }
}
