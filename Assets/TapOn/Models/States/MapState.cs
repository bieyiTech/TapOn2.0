using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;
using TencentMap.CoordinateSystem;

namespace TapOn.Models.States
{
    public class MapState
    {
        public Queue<Mark> marks;
        public List<Mark> marksJustLoading;
        public Queue<GameObject> marksOnMap;
        public double zoomLevel;

        public Coordinate upper;
        public Coordinate lower;
        public bool markLoading;
        public float moveSpeed;
        public float offsetX;
        public float offsetY;
        public float scale;
    }
}
