using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;

namespace TapOn.Models.States
{
    public class NewState
    {
        public string name;
    }
    public class AppState
    {
        public NewState newState;
        public MapState mapState;

        public static AppState initialState()
        {
            return new AppState
            {
                newState = new NewState
                {
                    name = "name",
                },
                mapState = new MapState
                {
                    upper = new Coordinate(),
                    lower = new Coordinate(),
                    marks = new List<Mark>(),
                    markLoading = false,
                }
            };
        }
    }
}
