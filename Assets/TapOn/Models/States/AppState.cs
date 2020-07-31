﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.widgets;
using TapOn.Constants;

namespace TapOn.Models.States
{
    public class AppState
    {
        public MapState mapState;
        public SettingState settingState;

        public static AppState initialState()
        {
            return new AppState
            {
                mapState = new MapState
                {
                    upper = new Coordinate(),
                    lower = new Coordinate(),
                    marks = new Queue<Mark>(),
                    marksJustLoading = new List<Mark>(),
                    marksOnMap = new Queue<GameObject>(),
                    markLoading = false,
                    moveSpeed = 300,
                    offsetX = 0,
                    offsetY = 0,
                    scaleLastFrame = 1,
                    scale = 1
                },
                settingState = new SettingState
                {
                    allIcons = new List<IconData>()
                    {
                        MyIcons.word_mine,
                        MyIcons.image_mine,
                        MyIcons.video_mine,
                        MyIcons.model_mine,
                    },
                    //index = 0,
                    products = new Queue<Prop>(),
                    objects = new List<GameObject>(),
                    models = new List<Model>(),
                    productSpan = true,
                    productShow = true,
                    productAppear = new List<bool> { false, false, false},
                    cameraType = 0,
                    productIndex = new List<int> { 0, 1, 2},
                    moveByCircle = false,
                    modelIndex = 0,
                    modelsMessageReady = false,
                }
            };
        }
    }
}
