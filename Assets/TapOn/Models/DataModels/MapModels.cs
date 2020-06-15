using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapOn.Models.DataModels
{
    /// <summary>
    /// latitude and longitude, maybe defined in sdk
    /// </summary>
    public class Coordinate
    {
        double latitude;
        double longitude;
    }
    /// <summary>
    /// the user's mark on map
    /// </summary>
    public class Mark
    {
        Coordinate coordinate;
    }
}
