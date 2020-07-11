using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Models.States
{
    public class SettingState
    {
        public List<IconData> allIcons;
        public Queue<Product> products;
        public List<GameObject> objects;
        //public List<MyIcon> icons;
        //public Texture2D sourceImage;
        //public string text;
        //public string videoPath;
        public int index;
        //public short rawIndex;
    }
}
