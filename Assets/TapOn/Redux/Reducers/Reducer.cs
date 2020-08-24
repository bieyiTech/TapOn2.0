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
using AREffect;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.async;

namespace TapOn.Redux.Reducers {
    public static class AppReducer
    {
        public static AppState Reduce(AppState state, object bAction)
        {
            Vector3 imageTF = Vector3.one;
            float ratio = 1.0f;
            bool saveImgTf = true ;

            int markSize = 20;
            switch (bAction)
            {
                //map
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
                        if (saveImgTf)
                        {
                            saveImgTf = false;
                            imageTF = instance.transform.Find("Cube").localScale;
                            ratio = imageTF.y * 1.0f / imageTF.x;
                        }
                        float ratio_tex = action.texture.height * 1.0f / action.texture.width;
                        Debug.Log("ratio: " + ratio + " ratio_text: " + ratio_tex);
                        
                        tf.localScale = ratio_tex > ratio ?
                            new Vector3(ratio / ratio_tex * imageTF.x, imageTF.y, imageTF.z) :
                            new Vector3(imageTF.x, ratio_tex / ratio * imageTF.y, imageTF.z);


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
                        Ray ray = Globals.instance.mapCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            
                            GameObject gameobj = hit.collider.gameObject;
                            Debug.Log(gameobj.tag);
                            if (gameobj.tag == "mark")
                            {
                                Globals.instance.contextStack.Push(Globals.instance.homeContext);
                                Navigator.push(Globals.instance.homeContext, new MaterialPageRoute(builder: (_) =>
                                {
                                    Globals.instance.map.SetActive(false);
                                    GameObject[] t = GameObject.FindGameObjectsWithTag("mark");
                                    foreach (GameObject mark in t)
                                        mark.SetActive(false);
                                    //Çëuse gameobj.GetComponent<LocationInfo>().mark
                                    Globals.instance.arEffect.SetActive(true);
                                    Window.instance.startCoroutine(Globals.instance.arEffect.GetComponent<AREffectManager>().PreviewMap(gameobj.GetComponent<LocationInfo>().mark));
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
                // setting
                case SetModelsMessageAction action:
                    {
                        state.settingState.models = action.models;
                        break;
                    }
                case ChangeModelProgressByIndexAction action:
                    {
                        state.settingState.models[action.index].progress = action.progress;
                        break;
                    }
                case ChangeModelLocalStateByIndexAction action:
                    {
                        state.settingState.models[action.index].isLocal = action.state;
                        break;
                    }
                case ChangeModelDownloadingStateByIndexAction action:
                    {
                        state.settingState.models[action.index].Downloading = action.state;
                        break;
                    }
                case AddProductAction action:
                    {
                        Debug.Log("S : " + action.product.text);
                        if (state.settingState.products.Count >= 3)
                            state.settingState.products.Dequeue();
                        state.settingState.products.Enqueue(action.product);
                        Debug.Log("nowLength: " + state.settingState.products.Count);
                        foreach (Prop prop in state.settingState.products)
                        {
                            Debug.Log(prop.type);
                            Debug.Log(prop.instance.tag);
                        }
                        break;
                    }
                case ChangeSpanStateAction action:
                    {
                        state.settingState.productSpan = action.state;
                        break;
                    }
                case ChangeShowStateAction action:
                    {
                        state.settingState.productShow = action.state;
                        break;
                    }
                case ChangeAppearStateAction action:
                    {
                        state.settingState.productAppear[action.index] = action.state;
                        break;
                    }
                case ChangeProductIndexAction action:
                    {
                        int temp = state.settingState.productIndex[2];
                        state.settingState.productIndex[2] = state.settingState.productIndex[1];
                        state.settingState.productIndex[1] = state.settingState.productIndex[0];
                        state.settingState.productIndex[0] = temp;
                        break;
                    }
                case ChangeMovebycircleStateAction action:
                    {
                        state.settingState.moveByCircle = action.state;
                        break;
                    }
                case ChangeModelIndexAction action:
                    {
                        state.settingState.modelIndex = action.index;
                        break;
                    }
                case ChangeModelMessageReadyStateAction action:
                    {
                        state.settingState.modelsMessageReady = action.state;
                        break;
                    }
                case ChangeCameraTypeAction action:
                    {
                        state.settingState.cameraType = action.CameraType;
                        break;
                    }
            }
            return state;
        }
    }
}