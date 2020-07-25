using System;
using System.Collections.Generic;
using System.Linq;
using TapOn.Redux.Actions;
using TapOn.Models.States;
using UnityEngine;
using TapOn.Api;
using TapOn.Models.DataModels;
using TapOn.Constants;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.material;
using TapOn.Screens;
using Unity.UIWidgets.Redux;

namespace TapOn.Redux.Reducers {
    public static class AppReducer
    {
        public static AppState Reduce(AppState state, object bAction)
        {
            int markSize = 20;
            switch (bAction)
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
                        state.mapState.scaleLastFrame = state.mapState.scale;
                        state.mapState.scale = action.scale;
                        break;
                    }
                case ChangeMapZoomLevelAction action:
                    {
                        state.mapState.zoomLevel = action.zoomLevel;
                        break;
                    }
                case ChangeIndexAction action:
                    {
                        state.settingState.index = action.index;
                        int ind = action.index;
                        switch(action.index)
                        {
                            case 0:
                                {
                                    break;
                                }
                            case 1:
                                {
                                    NativeCall.OpenPhoto((Texture2D tex) =>
                                    {
                                        //rawImage.texture = tex;
                                        //rawImage.rectTransform.sizeDelta = new Vector2(tex.width / 5, tex.height / 5);
                                    });
                                    break;
                                }
                            case 11:
                                {
                                    ind = 1;
                                    NativeCall.OpenCamera((Texture2D tex) =>
                                    {
                                        //rawImage.texture = tex;
                                        //rawImage.rectTransform.sizeDelta = new Vector2(tex.width/5, tex.height/5);
                                    });
                                    break;
                                }
                        }
                        if (state.settingState.products.Count >= 3)
                            state.settingState.products.Dequeue();
                        state.settingState.products.Enqueue(new Product() { type = (ProductType)(ind) });
                        break;
                    }
                case AddTextProductAction action:
                    {
                        GameObject instance = Prefabs.instance.templetes[0];
                        TextMesh tm = instance.GetComponentInChildren<TextMesh>();
                        tm.text = action.text;
                        Product product = new Product { type = ProductType.Text, instance = instance };
                        if (state.settingState.products.Count >= 3)
                            state.settingState.products.Dequeue();
                        state.settingState.products.Enqueue(product);
                        break;
                    }
                case AddImageProductAction action:
                    {
                        GameObject instance = Prefabs.instance.templetes[1];
                        Renderer rd = instance.GetComponentInChildren<Renderer>();
                        rd.material.mainTexture = action.texture;
                        Product product = new Product { type = ProductType.Text, instance = instance };
                        if (state.settingState.products.Count >= 3)
                            state.settingState.products.Dequeue();
                        state.settingState.products.Enqueue(product);
                        break;
                    }
                case AddMarkInViewAction action:
                    {
                        foreach(Mark mark in action.newMarks)
                        {
                            if(state.mapState.marks.Count > markSize)
                            {
                                state.mapState.marks.Dequeue();
                            }  
                            state.mapState.marks.Enqueue(mark);
                        }
                        break;
                    }
                case AddMarkJustLoadingAction action:
                    {
                        state.mapState.marksJustLoading = action.newMarks;
                        break;
                    }
                case AddMarkOnMapAction action:
                    {
                        foreach (GameObject mark in action.newMarks)
                        {
                            if (state.mapState.marksOnMap.Count > markSize)
                            {
                                GameObject t = state.mapState.marksOnMap.Dequeue();
                                UnityEngine.Object.Destroy(t);
                            }
                            state.mapState.marksOnMap.Enqueue(mark);
                        }
                        break;
                    }
                case SelectMarkAction action:
                    {
                        //Debug.LogError("pos: " + action.pos.x + " " + action.pos.y);
                        //Debug.LogError("pos2: " + Input.mousePosition.x + " " + Input.mousePosition.y);
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            //Debug.DrawLine(ray.origin, hit.point);
                            GameObject gameobj = hit.collider.gameObject;;
                            if (gameobj.tag == "mark")
                            {
                                Navigator.push(Prefabs.instance.homeContext, new MaterialPageRoute(builder: (_) =>
                                {
                                    return new StoreProvider<AppState>(
                                        store: StoreProvider.store,
                                        new MaterialApp(
                                            home: new FindScreenConnector()
                                        )
                                    );
                                }));
                            }
                        }
                        break;
                    }
                case SetModelsMessageAction action:
                    {
                        state.settingState.models = action.models;
                        break;
                    }
            }
            return state;
        }
    }
}