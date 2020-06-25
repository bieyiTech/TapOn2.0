using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;
using TencentMap.CoordinateSystem;

namespace TapOn.Models.States
{
    public class AppState
    {
        public MapState mapState;
        public SettingState settingState;

        public static AppState initialState()
        {
            List<Mark> mapMarks = new List<Mark>();
            mapMarks.Add(new Mark { coordinate = new Coordinate(39.986, 116.308) });
            mapMarks.Add(new Mark { coordinate = new Coordinate(39.983, 116.309) });
            return new AppState
            {
                mapState = new MapState
                {
                    upper = new Coordinate(),
                    lower = new Coordinate(),
                    marks = mapMarks,
                    positions = new List<Vector2>(),
                    pixelPositions = new List<Position>(),
                    markLoading = false,
                    moveSpeed = 300,
                    offsetX = 0,
                    offsetY = 0,
                    scale = 1
                },
                settingState = new SettingState
                {
                    allIcons = new List<List<MyIcon>>(),
                    index = 0,
                    sourceImage = null,
                    text = "",
                    videoPath = ""
                }
            };
        }
    }
}
