using System.Collections;
using System.Collections.Generic;
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Models.DataModels
{
    /// <summary>
    /// latitude and longitude, maybe defined in sdk
    /// </summary>
    /*public class Coordinate
    {
        double latitude;
        double longitude;
    }*/
    /// <summary>
    /// the user's mark on map
    /// </summary>
    public class Mark
    {
        public string mapId;
        public string mapName;
        public List<BaseProp> props;
        public string id;
        public Coordinate coordinate;
        public string date;
        public string url;
        public string fileName;
    }

    public class Position
    {
        public float top;
        public float bottom;
        public float left;
        public float right;
    }
}
