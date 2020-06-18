using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using UnityEngine;

namespace TapOn.Models.States
{
    public class SettingState
    {
        public List<List<MyIcon>> allIcons;
        //public List<MyIcon> icons;
        public Texture2D sourceImage;
        public string text;
        public string videoPath;
        public short index;
        //public short rawIndex;
    }
}
