using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;
using TencentMap.CoordinateSystem;

namespace TapOn.Models.States
{
    public class MapState
    {
        public List<Mark> marks;
        public List<Vector2> positions;
        public List<Position> pixelPositions;
        public Coordinate upper;
        public Coordinate lower;
        public bool markLoading;
        public float moveSpeed;
        public float offsetX;
        public float offsetY;
        public float scale;
    }
}
