﻿using cn.bmob.response;
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

    public class ChangeMapZoomLevelAction
    {
        public double zoomLevel;
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
                var offsetY = getState().mapState.offsetY;
                return MapApi.MoveMap(offsetX, offsetY);
            });
        }

        public static object zoomMap()
        {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                var scale = getState().mapState.scale;
                var scaleLastFrame = getState().mapState.scaleLastFrame;
                return MapApi.ZoomMap(scale, scaleLastFrame);
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
                        if (_mark.objectId.Equals(mark.objectId))                        {
                            isNew = false;
                            break;
                        }
                    }
                    if(isNew)
                    {
                        newMarks.Add(_mark);
                    }
                }
                return MapApi.AddMark(newMarks).Then((list) =>
                {
                    dispatcher.dispatch(new AddMarkOnMapAction { newMarks = list });
                    for(int i = 0;i < newMarks.Count; i++)
                    {
                        newMarks[i].logoInstance = list[i];
                    }
                    dispatcher.dispatch(new AddMarkInViewAction { newMarks = newMarks });
                }).Catch((ex) =>
                {
                    Debug.LogError(ex.Message);
                });
            });
            
        }
    }
}
