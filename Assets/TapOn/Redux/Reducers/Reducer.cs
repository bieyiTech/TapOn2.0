using System;
using System.Collections.Generic;
using System.Linq;
using TapOn.Redux.Actions;
using TapOn.Models.States;
using UnityEngine;
using TapOn.Api;

namespace TapOn.Redux.Reducers {
    public static class AppReducer
    {
        public static AppState Reduce(AppState state, object bAction)
        {
            switch(bAction)
            {
                case MapHorizontalDragAction action:
                    {
                        state.mapState.offsetX = action.offset;
                        break;
                    }
                case MapVerticalDragAction action:
                    {
                        state.mapState.offsetY = action.offset;
                        break;
                    }
                case MapZoomAction action:
                    {
                        state.mapState.scale = action.scale;
                        break;
                    }
                case UpdatePositionsAction action:
                    {
                        if(!action.update)
                        {
                            state.mapState.positions = new List<Vector2>();
                            break;
                        }
                        for (int i = 0; i < state.mapState.marks.Count; i++)
                        {
                            if (state.mapState.positions.Count > i)
                                state.mapState.positions[i] = MapApi.CoordinateConvert(state.mapState.marks[i].coordinate, 40);
                            else
                                state.mapState.positions.Add(MapApi.CoordinateConvert(state.mapState.marks[i].coordinate, 40));
                        }
                        //state.mapState.positions = action.positions;
                        break;
                    }
                case UpdatePixelPositionsAction action:
                    {
                        state.mapState.pixelPositions = action.positions;
                        break;
                    }
            }
            return state;
        }
    }
}