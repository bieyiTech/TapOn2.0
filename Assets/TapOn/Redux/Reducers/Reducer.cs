using System;
using System.Collections.Generic;
using System.Linq;
//using TapOn.Redux.Actions;
using TapOn.Models.States;

namespace TapOn.Redux.Reducers {
    public static class AppReducer
    {
        public static AppState Reduce(AppState state, object bAction)
        {
            return new AppState();
        }
    }
}