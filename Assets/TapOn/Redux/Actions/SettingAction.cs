using System.Collections;
using System.Collections.Generic;
using TapOn.Api;
using TapOn.Constants;
using TapOn.Utils;
using TapOn.Models.DataModels;
using TapOn.Models.States;
using Unity.UIWidgets.async;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using UnityEngine;

namespace TapOn.Redux.Actions
{
    public class AddProductAction
    {
        public Prop product;
    }

    public class AddTextProductAction
    {
        public string text;
    }

    public class AddImageProductAction
    {
        public Texture2D texture;
    }

    public class AddVideoProductAction
    {
        public string path;
    }

    public class SetModelsMessageAction
    {
        public List<Model> models;
    }

    public class ChangeSpanStateAction
    {
        public bool state;
    }

    public class ChangeShowStateAction
    {
        public bool state;
    }

    public class ChangeAppearStateAction
    {
        public bool state;
        public int index;
    }

    public class ChangeProductIndexAction
    {
    }

    public class ChangeMovebycircleStateAction
    {
        public bool state;
    }

    public class ChangeModelIndexAction
    {
        public int index;
    }

    public class ChangeModelMessageReadyStateAction
    {
        public bool state;
    }

    public class ChangeCameraTypeAction
    {
        public int CameraType;
    }

    public static partial class Actions
    {
        public static object AddTextProduct(string text)
        {
            return new ThunkAction<AppState>((dispatcher, getState) =>
            {
                GameObject instance = Globals.instance.templetes[0];
                TextMesh tm = instance.GetComponentInChildren<TextMesh>();
                tm.text = text;
                Prop product = new Prop { type = (int)ProductType.Text, text = text, instance = instance };
                if(getState().settingState.products.Count < 3)
                {
                    dispatcher.dispatch(new AddProductAction { product = product });
                    dispatcher.dispatch(new ChangeAppearStateAction { state = true, index = getState().settingState.products.Count -1 });
                    return 0;
                }
                dispatcher.dispatch(new ChangeProductIndexAction());
                dispatcher.dispatch(new ChangeAppearStateAction { state = false, index = 2 });
                Window.instance.startCoroutine(
                  TapOnUtils.WaitSomeTime(
                      time: 0.3f,
                      after: () =>
                      {
                      dispatcher.dispatch(new AddProductAction { product = product });
                      Window.instance.startCoroutine(
                        TapOnUtils.WaitSomeTime(
                            time: 0.3f,
                            after: (() => 
                            {dispatcher.dispatch(new ChangeAppearStateAction { state = true, index = 2 }); })));
                       }
                       ));
                return 0;
            });
        }

        public static object AddImageProduct(Texture2D texture)
        {
            return new ThunkAction<AppState>((dispatcher, getState) =>
            {
                GameObject instance = Globals.instance.templetes[1];
                Renderer rd = instance.GetComponentInChildren<Renderer>();
                rd.material.mainTexture = texture;
                Prop product = new Prop { type = (int)ProductType.Picture, texture_byte = texture.EncodeToPNG(), instance = instance };
                if (getState().settingState.products.Count < 3)
                {
                    dispatcher.dispatch(new AddProductAction { product = product });
                    dispatcher.dispatch(new ChangeAppearStateAction { state = true, index = getState().settingState.products.Count - 1 });
                    return 0;
                }
                dispatcher.dispatch(new ChangeProductIndexAction());
                dispatcher.dispatch(new ChangeAppearStateAction { state = false, index = 2 });
                Window.instance.startCoroutine(
                  TapOnUtils.WaitSomeTime(
                      time: 0.3f,
                      after: () =>
                      {
                          dispatcher.dispatch(new AddProductAction { product = product });
                          Window.instance.startCoroutine(
                            TapOnUtils.WaitSomeTime(
                                time: 0.3f,
                                after: (() =>
                                { dispatcher.dispatch(new ChangeAppearStateAction { state = true, index = 2 }); })));
                      }
                       ));
                return 0;
            });
        }
    }
}
