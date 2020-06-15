using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models.State
{
    public class NewState
    {
        public string name;
    }
    public class AppState
    {
        public NewState newState;

        public static AppState initialState()
        {
            return new AppState
            {
                newState = new NewState
                {
                    name = "name",
                }
            };
        }
    }
}
