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
                case AddTextProductAction action:
                    {
                        GameObject instance = Globals.instance.templetes[0];
                        TextMesh tm = instance.GetComponentInChildren<TextMesh>();
                        tm.text = action.text;
                        Prop product = new Prop { type = (int)ProductType.Text, text = action.text, instance = instance };
                        if (state.settingState.products.Count >= 3)
                            state.settingState.products.Dequeue();
                        state.settingState.products.Enqueue(product);
                        break;
                    }
                case AddImageProductAction action:
                    {
                        GameObject instance = Globals.instance.templetes[1];
                        Renderer rd = instance.GetComponentInChildren<Renderer>();
                        UnityEngine.Transform tf = instance.transform.Find("Cube");
                        float ratio = tf.localScale.y * 1.0f / tf.localScale.x;
                        float ratio_tex = action.texture.height * 1.0f / action.texture.width;
                        Debug.Log("ratio: " + ratio + " ratio_text: " + ratio_tex);
                        
                        tf.localScale = ratio_tex > ratio ?
                            new Vector3(ratio / ratio_tex * tf.localScale.x, tf.localScale.y, tf.localScale.z) :
                            new Vector3(tf.localScale.x, ratio_tex / ratio * tf.localScale.y, tf.localScale.z);


                        rd.material.mainTexture = action.texture;
                        Prop product = new Prop { type = (int)ProductType.Picture, texture_byte = action.texture.EncodeToPNG(), instance = instance };
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
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            GameObject gameobj = hit.collider.gameObject;;
                            if (gameobj.tag == "mark")
                            {
                                Globals.instance.contextStack.Push(Globals.instance.homeContext);
                                Navigator.push(Globals.instance.homeContext, new MaterialPageRoute(builder: (_) =>
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