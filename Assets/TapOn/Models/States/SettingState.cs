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
        public Queue<Prop> products;

        public List<Model> models;

        public List<GameObject> objects;

        //圆盘展开状态
        public bool productSpan;
        //圆盘显示状态（圆盘收拢时不可见，否则影响按下判定）
        public bool productShow;
        //转盘的索引顺序（123->231）
        public List<int> productIndex;
        //三个转轮的显隐控制
        public List<bool> productAppear;
        //使用轮盘移动
        public bool moveByCircle;
        //选择的模型类型，0为全部
        public int modelIndex;
        //模型信息是否下拉完成
        public bool modelsMessageReady;
        //相机类型 0创作 1照片 2视频
        public int cameraType;
    }
}
