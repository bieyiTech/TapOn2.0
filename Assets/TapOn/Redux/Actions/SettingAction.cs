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
        static Vector3 imageTF = Vector3.one;
        static float ratio = 1.0f;
        static bool saveImgTf = true;

        public static object AddTextProduct(string text)
        {
            return new ThunkAction<AppState>((dispatcher, getState) =>
            {
                GameObject instance = GameObject.Instantiate(Globals.instance.templetes[0]);
                instance.tag = "word";
                instance.SetActive(false);
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

        public static object AddImageProduct(Texture2D texture, Unity.UIWidgets.widgets.BuildContext context)
        {
            return new ThunkAction<AppState>((dispatcher, getState) =>
            {
                GameObject instance = GameObject.Instantiate(Globals.instance.templetes[1]);
                instance.tag = "texture";
                instance.SetActive(false);
                Renderer rd = instance.GetComponentInChildren<Renderer>();

                UnityEngine.Transform tf = instance.transform.Find("Cube");
                if (saveImgTf)
                {
                    saveImgTf = false;
                    imageTF = instance.transform.Find("Cube").localScale;
                    ratio = imageTF.y * 1.0f / imageTF.x;
                }
                float ratio_tex = texture.height * 1.0f / texture.width;
                Debug.Log("ratio: " + ratio + " ratio_text: " + ratio_tex);

                tf.localScale = ratio_tex > ratio ?
                    new Vector3(ratio / ratio_tex * imageTF.x, imageTF.y, imageTF.z) :
                    new Vector3(imageTF.x, ratio_tex / ratio * imageTF.y, imageTF.z);

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
                if (Window.instance == null) Debug.Log("null");
                using (Unity.UIWidgets.widgets.WindowProvider.of(context).getScope())
                {
                    Window.instance.startCoroutine(
                      TapOnUtils.WaitSomeTime(
                          time: 0.3f,
                          after: () =>
                          {
                              Debug.Log("xs");
                              dispatcher.dispatch(new AddProductAction { product = product });
                              Window.instance.startCoroutine(
                                TapOnUtils.WaitSomeTime(
                                    time: 0.3f,
                                    after: (() =>
                                    { dispatcher.dispatch(new ChangeAppearStateAction { state = true, index = 2 }); })));
                          }
                           ));
                }
                return 0;
            });
        }
    }
}
