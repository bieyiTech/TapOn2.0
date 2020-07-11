using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using UnityEngine;

namespace TapOn.Models.ViewModels
{
    public class MapScreenViewModel
    {
        public Queue<Mark> marks;
        public List<Vector2> positions;
        public List<Position> pixelPositions;
    }
}
