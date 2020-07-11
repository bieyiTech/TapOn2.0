using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using UnityEngine;

namespace TapOn.Models.ActionModels
{
    public class MapScreenActionModel
    {
        public Action<float> mapMoveOffsetX;
        public Action<float> mapMoveOffsetY;
        public Action<float> mapZoom;
        public Action<double> changeZoomLevel;
        public Action<List<Mark>> addMarkJustLoading;
        public Action<Vector2> selectMark;
        public Func<IPromise> moveMap;
        public Func<IPromise> zoomMap;
        public Func<IPromise> changeMark;
        public Func<IPromise> loadMark;
    }
}
