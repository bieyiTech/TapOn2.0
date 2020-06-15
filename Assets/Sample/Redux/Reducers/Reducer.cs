using System;
using System.Collections.Generic;
using System.Linq;
using Redux.Actions;
using Models.State;

namespace Redux.Reducers {
    public static class AppReducer
    {
        static readonly List<string> _nonce = new List<string>();

        public static AppState Reduce(AppState state, object bAction)
        {
            switch (bAction)
            {
                case ChangeNameAction action:
                    {
                        state.newState.name = action.changeName;
                        break;
                    }
            }
            return state;
        }
    }
}