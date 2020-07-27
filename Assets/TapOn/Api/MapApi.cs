using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using TapOn.Constants;
using TapOn.Models.DataModels;
using TencentMap.API;
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.ui;
using UnityEngine;


namespace TapOn.Api
{
    public static class MapApi
    {
        public static MapController map { get { return Globals.instance.mapController; } }

        public static MapEnd mapEnd;
        public static Camera camera;

        public static Promise<string> MoveMap(float offX, float offY)
        {
            var promise = new Promise<string>();
            map.MoveOffset(offX * 2.75, offY * 2.75);
            map.DidRender();
            promise.Resolve(value: "move success!");
            return promise;
        }

        public static Promise<string> ZoomMap(float scale, float scaleLastFrame)
        {
            var promise = new Promise<string>();
            if (scale == 1) return promise;
            float nowLevel = (float)map.GetZoomLevel();
            float newLevel;
            float s = scale / scaleLastFrame;
            float deltaLevel = Mathf.Log(s, 2) * 0.5f;
            Debug.Log("Scale: " + scale);
            Debug.Log("deltaLevel: " + deltaLevel);
            newLevel = nowLevel + deltaLevel;
            newLevel = Mathf.Clamp(newLevel, 4, 16);
            map.SetZoomLevel(newLevel);
            map.DidRender();
            promise.Resolve(value: "zoom success!");
            return promise;
        }

        public static Promise<List<GameObject>> AddMark(List<Mark> marks)
        {
            var promise = new Promise<List<GameObject>>(true);
            List<GameObject> m = new List<GameObject>();

            foreach(Mark mark in marks)
            {
                Globals.instance.markManager.AddMark(mark);
            }
            
            promise.Resolve(value: m);
            return promise;
        }

        public static Vector2 CoordinateConvert(Coordinate coordinate, float size)
        {
            if (map == null) return new Vector2(-0.5f, -0.5f);
            Vector2 newPos = map.ConvertCoordinateToScreen(coordinate);
            //return newPos;
            return new Vector2((newPos.x - 0.5f * size) / (Screen.width - size), (Screen.height - newPos.y - size) / (Screen.height - size));
            //Vector2 rate = new Vector2((newPos.x - size/2) / (Screen.width - size), (Screen.height - newPos.y - size + 0.5f * size) / (Screen.height - size));
            //Vector2 rate = new Vector2((newPos.x) / (Screen.width), (Screen.height - newPos.y) / (Screen.height));

            //return rate;
        }

        public static Position CoordinateConvertLTRB(Coordinate coordinate, int size)
        {
            if (map == null) return new Position {bottom = 0,left = 0,top = 0, right = 0 };
            Vector2 newPos = map.ConvertCoordinateToScreen(coordinate);
            return new Position { bottom = newPos.y, left = newPos.x - size * 0.5f, top = Screen.height - newPos.y - size, right = Screen.width - newPos.x - size * 0.5f };
        }
    }
}
