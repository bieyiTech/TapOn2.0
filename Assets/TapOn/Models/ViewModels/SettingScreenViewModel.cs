using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Models.ViewModels
{
    public class SettingScreenViewModel
    {
        public Product[] products;
        public List<IconData> allIcons;
        public List<GameObject> objects;
        public List<Model> models;
        public int index;
        public bool isScanning;
    }
}
