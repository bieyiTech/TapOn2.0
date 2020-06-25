using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapOn.Models.ActionModels
{
    public class MapScreenActionModel
    {
        public Action<float> mapMoveOffsetX;
        public Action<float> mapMoveOffsetY;
        public Action<float> mapZoom;
        public Action<bool> markPositionUpdate;
        public Func<IPromise> moveMap;
        public Func<IPromise> zoomMap;
    }
}
