using cn.bmob.response;
using System.Collections;
using System.Collections.Generic;
using TapOn.Api;
using TapOn.Models;
using TapOn.Models.DataModels;
using TapOn.Models.States;
using TencentMap.API;
using TencentMap.CoordinateSystem;
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

    public class AddMarkJustLoadingAction
    {
        public List<Mark> newMarks;
    }

    public class AddMarkInViewAction
    {
        public List<Mark> newMarks;
    }

    public class AddMarkOnMapAction
    {
        public List<GameObject> newMarks;
    }

    public class SelectMarkAction
    {
        public Vector2 pos;
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
                    //to do 获取新标记
                    //List<Mark> marks = new List<Mark>();
                    
                    //mapMarks.Add(new Mark { coordinate = new Coordinate(39.986, 116.308) });
                    //mapMarks.Add(new Mark { coordinate = new Coordinate(39.983, 116.309) });
                    
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

        public static object changeMark()
        {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                List<Mark> newMarks = new List<Mark>();
                foreach (Mark _mark in getState().mapState.marksJustLoading)
                {
                    bool isNew = true;
                    foreach(Mark mark in getState().mapState.marks)
                    {
                        if (_mark.id.Equals(mark.id))                        {
                            isNew = false;
                            break;
                        }
                    }
                    if(isNew)
                    {
                        newMarks.Add(_mark);
                    }
                }
                dispatcher.dispatch(new AddMarkInViewAction { newMarks = newMarks });
                return MapApi.AddMark(newMarks).Then((list) =>
                {
                    dispatcher.dispatch(new AddMarkOnMapAction { newMarks = list });
                }).Catch((ex) =>
                {
                    Debug.LogError(ex.Message);
                });
            });
            
        }

        public static object loadMark()
        {
            return new ThunkAction<AppState>((dispatcher, getState) =>
            {
                //QueryCallbackData<Marks> data = await BmobApi.queryFuzztMarksAsync(MapApi.map.GetCoordinate(), 3);
                List<Mark> mapMarks = new List<Mark>();
                return BmobApi.queryFuzzyMarks(MapApi.map.GetCoordinate(), 3).Then((list) =>
                {
                    Debug.LogError("mark count: " + list.Count);
                    mapMarks = list;
                    dispatcher.dispatch(new AddMarkJustLoadingAction { newMarks = mapMarks });
                    dispatcher.dispatch(changeMark());
                });
            });
        }
    }
}
