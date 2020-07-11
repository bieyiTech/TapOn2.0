using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.widgets;
using TapOn.Constants;

namespace TapOn.Models.States
{
    public class AppState
    {
        public MapState mapState;
        public SettingState settingState;

        public static AppState initialState()
        {
            return new AppState
            {
                mapState = new MapState
                {
                    upper = new Coordinate(),
                    lower = new Coordinate(),
                    marks = new Queue<Mark>(),
                    marksJustLoading = new List<Mark>(),
                    marksOnMap = new Queue<GameObject>(),
                    markLoading = false,
                    moveSpeed = 300,
                    offsetX = 0,
                    offsetY = 0,
                    scale = 1
                },
                settingState = new SettingState
                {
                    allIcons = new List<IconData>()
                    {
                        MyIcons.text,
                        MyIcons.picture,
                        MyIcons.video,
                        MyIcons.UnityLogo,
                    },
                    index = 0,
                    products = new Queue<Product>(),
                    objects = new List<GameObject>(),
                }
            };
        }
    }
}
