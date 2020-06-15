using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;

namespace TapOn.Models.States
{
    public class MapState
    {
        public List<Mark> marks;
        public Coordinate upper;
        public Coordinate lower;
        public bool markLoading;
    }
}
