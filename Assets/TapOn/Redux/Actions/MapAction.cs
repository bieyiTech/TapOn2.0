using System.Collections;
using System.Collections.Generic;
using TapOn.Api;
using TapOn.Models.DataModels;
using TapOn.Models.States;
using TencentMap.API;
using Unity.UIWidgets.Redux;
using UnityEngine;

namespace TapOn.Redux.Actions
{
    public class MapVerticalDragAction
    {
        public float offset;
    }

    public class MapHorizontalDragAction
    {
        public float offset;
    }

    public class MapZoomAction
    {
        public float scale;
    }

    public class UpdatePositionsAction
    {
        public List<Vector2> positions;
        public bool update;
    }

    public class UpdatePixelPositionsAction
    {
        public List<Position> positions;
        public bool update;
    }

    public static partial class Actions
    {
        public static object moveMap()
        {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                var offsetX = getState().mapState.offsetX;
                //Debug.LogError("offsetX: " + offsetX);
                var offsetY = getState().mapState.offsetY;
                return MapApi.MoveMap(offsetX, offsetY)
                .Then(message=>
                {
                    List<Mark> marks = getState().mapState.marks;
                    /*List<Position> pos = new List<Position>();
                    for (int i = 0; i < marks.Count; i++)
                    {
                        pos.Add(MapApi.CoordinateConvertLTRB(marks[i].coordinate, 40));
                    }
                    dispatcher.dispatch(new UpdatePixelPositionsAction { positions = pos, });*/
                    List<Vector2> pos = new List<Vector2>();
                    List<Vector2> positions = getState().mapState.positions;

                    /*if (positions == null || positions.Count < 1)
                    {
                        for (int i = 0; i < marks.Count; i++)
                        {
                            pos.Add(MapApi.CoordinateConvert(marks[i].coordinate, 40));
                        }

                        dispatcher.dispatch(new UpdatePositionsAction { positions = pos, });
                    }*/
                    /*for(int i=0;i<marks.Count;i++)
                    {
                        pos.Add(MapApi.CoordinateConvert(marks[i].coordinate, 40));
                    }*/

                    //dispatcher.dispatch(new UpdatePositionsAction {positions = pos, update = false,});

                    /*else
                    {
                        for (int i = 0; i < positions.Count; i++)
                        {
                            pos.Add(new Vector2(positions[i].x - offsetX / (Screen.width-40),
                                positions[i].y + offsetY / (Screen.height-40)));
                        }
                        dispatcher.dispatch(new UpdatePositionsAction { positions = pos, });
                    }*/
                });
            });
        }

        public static object zoomMap()
        {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                var scale = getState().mapState.scale;
                return MapApi.ZoomMap(scale);
            });
        }
    }
}
