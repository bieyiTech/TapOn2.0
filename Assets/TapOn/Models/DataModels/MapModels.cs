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
        public Coordinate coordinate;
        public static IconData mark_icon = new IconData(0xe55f, "Material Icons");
    }

    public class Position
    {
        public float top;
        public float bottom;
        public float left;
        public float right;
    }
}
