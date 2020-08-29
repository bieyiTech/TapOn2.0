using RSG;
using System.Collections;
using System.Collections.Generic;
using TapOn.Constants;
using TapOn.Models.ActionModels;
using TapOn.Models.DataModels;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using TapOn.Redux.Actions;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.async;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Stack = Unity.UIWidgets.widgets.Stack;
using DialogUtils = Unity.UIWidgets.material.DialogUtils;
using cn.bmob.response;
using TapOn.Models;
using TapOn.Api;
using com.unity.uiwidgets.Runtime.rendering;
using TapOn.Redux;
using Unity.UIWidgets.cupertino;
using AREffect;
using TapOn.Utils;
using System.IO;

namespace TapOn.Screens
{
    public class SettingScreenConnector : StatelessWidget
    {
        public SettingScreenConnector(
            Key key = null
        ) : base(key: key)
        {
        }
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, SettingScreenViewModel>(
                converter: state =>
                {
                    return new SettingScreenViewModel
                    {
                        products = state.settingState.products.ToArray(),
                        allIcons = state.settingState.allIcons,
                        models = state.settingState.models,
                        productSpan = state.settingState.productSpan,
                        productShow = state.settingState.productShow,
                        productAppear = state.settingState.productAppear,
                        productIndex = state.settingState.productIndex,
                        moveByCircle = state.settingState.moveByCircle,
                        modelIndex = state.settingState.modelIndex,
                        modelsMessageReady = state.settingState.modelsMessageReady,
                        cameraType = state.settingState.cameraType,
                    };
                },
                builder: (context1, viewModel, dispatcher) =>
                {
                    var actionModel = new SettingScreenActionModel
                    {
                        AddTextProduct = (value) =>
                        { dispatcher.dispatch(new AddTextProductAction { text = value, }); },
                        AddTextProductFuc = (text) =>
                        { return dispatcher.dispatch<object>(Actions.AddTextProduct(text)); },
                        AddImageProduct = (value) =>
                        { dispatcher.dispatch(new AddImageProductAction { texture = value, }); },
                        AddImageProductFuc = (texture, cont) =>
                        { return dispatcher.dispatch<object>(Actions.AddImageProduct(texture, cont)); },
                        AddVideoProductFuc = (path, cont) =>
                        { return dispatcher.dispatch<object>(Actions.AddVideoProduct(path, cont)); },
                        AddModelProductFuc = (model, id) =>
                        { return dispatcher.dispatch<object>(Actions.AddModelProduct(model, id)); },
                        SetModelsMessage = (models) =>
                        { dispatcher.dispatch(new SetModelsMessageAction { models = models, }); },
                        ChangeModelLocalStateByIndex = (index, state) =>
                        { dispatcher.dispatch(new ChangeModelLocalStateByIndexAction { index = index, state = state }); },
                        ChangeModelDownloadingStateByIndex = (index, state) =>
                        { dispatcher.dispatch(new ChangeModelDownloadingStateByIndexAction { index = index, state = state }); },
                        ChangeModelProgressByIndex = (index, p) =>
                        { dispatcher.dispatch(new ChangeModelProgressByIndexAction { index = index, progress = p }); },
                        ChangeSpanState = (state) =>
                        { dispatcher.dispatch(new ChangeSpanStateAction { state = state, }); },
                        ChangeShowState = (state) =>
                        { dispatcher.dispatch(new ChangeShowStateAction { state = state, }); },
                        ChangeAppearState = (state, index) =>
                        { dispatcher.dispatch(new ChangeAppearStateAction { state = state, index = index, }); },
                        ChangeProductIndex = () =>
                        { dispatcher.dispatch(new ChangeProductIndexAction()); },
                        ChangeModelIndex = (index) =>
                        { dispatcher.dispatch(new ChangeModelIndexAction {index = index, }); },
                        ChangeModelMessageReadyState = (state) =>
                        { dispatcher.dispatch(new ChangeModelMessageReadyStateAction { state = state, }); },
                        ChangeCameraType = (index) =>
                        { dispatcher.dispatch(new ChangeCameraTypeAction { CameraType = index, }); },

                    };
                    return new SettingScreen(viewModel: viewModel, actionModel: actionModel);
                }
            );
        }
    }

    public class SettingScreen : StatefulWidget
    {
        public SettingScreen(
        SettingScreenViewModel viewModel = null,
        SettingScreenActionModel actionModel = null,
        Key key = null
            ) : base(key: key)
        {
            this.viewModel = viewModel;
            this.actionModel = actionModel;
        }

        public readonly SettingScreenViewModel viewModel;
        public readonly SettingScreenActionModel actionModel;

        public override State createState()
        {
            _SettingScreenState s = new _SettingScreenState();
            return s;
        }
    }

    public class _SettingScreenState : SingleTickerProviderStateMixin<SettingScreen>
    {
        byte[] snapChatForCza;

        List<float> left = new List<float> { 0, 56.6f, 80 };
        List<float> bottom = new List<float> { 80, 56.6f, 0 };

        List<bool> drag = new List<bool> {false, false, false };
        float circleDragLeft = 0;
        float circleDragBottom = 0;

        PageController modelsPc = new PageController(initialPage: 0);

        bool modelsPreviewReady = false;

        bool settingDown = false;
        float pointCloudProgress = 0.5f;

        BuildContext bottomSheetContext;

        public override void initState()
        {
            base.initState();
            if (widget.viewModel.models.Count == 0)
                getModelsMessage();
        }

        private async void getModelsMessage()
        {
            for (int i = 0; i < 3; i++)
            {
                QueryCallbackData<BmobModel> data = await BmobApi.queryAllModelsMessage();
                Debug.Log(data.ToString());
                if (data != null)
                {
                    List<Model> models = new List<Model>();
                    foreach (var model in data.results)
                    {
                        Model m = new Model
                        {
                            isLocal = false,
                            Downloading = false,
                            id = model.objectId,
                            progress = 0,
                            modelName = model.modelName,
                            modelType = model.modelType.Get(),
                            previewFileName = model.preview.filename,
                            previewUrl = model.preview.url,
                            assetName =  model.asset == null ? "" : model.asset.filename,
                            assetUrl = model.asset == null ? "" : model.asset.url,
                        };
                        FileInfo f = new FileInfo(Application.persistentDataPath + "//model//" + m.assetName);
                        if(f.Exists)
                        {
                            m.isLocal = true;
                            AssetBundle ab = AssetBundle.LoadFromFile(f.FullName);
                            GameObject sp = ab.LoadAsset<GameObject>(m.modelName);
                            Globals.instance.models.Add(m.id, sp);
                        }
                        models.Add(m);
                    }
                    this.widget.actionModel.SetModelsMessage(models);   
                    widget.actionModel.ChangeModelMessageReadyState(true);
                    if (bottomSheetContext != null)
                        ((StatefulElement)bottomSheetContext).markNeedsBuild();
                    break;
                }
            }
            
        }
        private IEnumerator wait_300()
        {
            yield return new UIWidgetsWaitForSeconds(0.3f);
            widget.actionModel.ChangeShowState(widget.viewModel.productSpan);
        }
        private Widget _modelsPreview(int index, BuildContext context)
        {
            List<Model> models = new List<Model>();
            List<int> indexs = new List<int>();
            if (index == 0)
            {
                models = widget.viewModel.models;
                indexs = new List<int> { 0, 1, 2, 3, 4, 5 };
            }
            else
            {
                int ii = 0;
                foreach (Model m in widget.viewModel.models)
                {
                    if (m.modelType == index)
                    {
                        models.Add(m);
                        indexs.Add(ii);
                    }
                    ii++;
                }
            }
            if (widget.viewModel.modelsMessageReady)
                return GridView.builder(
                    gridDelegate: new SliverGridDelegateWithFixedCrossAxisCount(
                        crossAxisCount: 4,
                        childAspectRatio: 1.0f
                        ),
                    itemCount: models.Count,
                    itemBuilder: (context_grid, ind) =>
                    {
                        return new GestureDetector(
                            onTap: () =>
                            {
                                if (models[ind].Downloading) return;
                                if (!models[ind].isLocal)
                                {
                                    widget.actionModel.ChangeModelDownloadingStateByIndex(indexs[ind], true);
                                    ((StatefulElement)bottomSheetContext).markNeedsBuild();
                                    Window.instance.startCoroutine(
                                        TapOnUtils.downloadModel(
                                            models[ind].assetUrl,
                                            models[ind].assetName,
                                            progress =>
                                            {
                                                widget.actionModel.ChangeModelProgressByIndex(indexs[ind], progress);
                                                ((StatefulElement)bottomSheetContext).markNeedsBuild();
                                            },
                                            request =>
                                            {
                                                //
                                                //AssetBundle ab = (request.downloadHandler as UnityEngine.Networking.DownloadHandlerAssetBundle).assetBundle;
                                                //AssetBundle ab = ((UnityEngine.Networking.DownloadHandlerAssetBundle)request.downloadHandler).assetBundle;
                                                //AssetBundle ab = AssetBundle.LoadFromMemoryAsync(request.downloadHandler.data).assetBundle;
                                                Stream sw;
                                                FileInfo t = new FileInfo(Application.persistentDataPath + "//model//" + models[ind].assetName);
                                                int length = request.downloadHandler.data.Length;
                                                byte[] bytes = request.downloadHandler.data;
                                                if (!t.Exists)
                                                { 
                                                    sw = t.Create();
                                                }
                                                else
                                                { 
                                                    sw = t.OpenWrite();
                                                }
                                                sw.Write(bytes, 0, length);
                                                sw.Close();
                                                sw.Dispose();
                                                AssetBundle ab = AssetBundle.LoadFromFile(t.FullName);
                                                GameObject sp = ab.LoadAsset<GameObject>(models[ind].modelName);
                                                Globals.instance.models.Add(models[ind].id, sp);
                                                //widget.actionModel.AddModelProductFuc(sp, models[ind].id);
                                                widget.actionModel.ChangeModelLocalStateByIndex(indexs[ind], true);
                                                widget.actionModel.ChangeModelDownloadingStateByIndex(indexs[ind], false);
                                                ((StatefulElement)bottomSheetContext).markNeedsBuild();
                                            //ab.Unload();
                                            //GameObject.Instantiate(sp);
                                        }));
                                }
                                else
                                {
                                    widget.actionModel.AddModelProductFuc(Globals.instance.models[models[ind].id], models[ind].id);
                                    Navigator.of(bottomSheetContext).pop(null);
                                }
                            },
                            child: new Stack(
                                children: new List<Widget>
                                {
                                    new Unity.UIWidgets.widgets.Image(image: new NetworkImage(url: models[ind].previewUrl)),
                                    new Align(alignment: Alignment.topRight, child: new Icon(icon: models[ind].isLocal ? MyIcons.check : MyIcons.keyboard_arrow_down, size: 20, color: CColors.Blue)),
                                    new Padding(padding: EdgeInsets.only(left: 10, right: 10), child: new Align(alignment: Alignment.bottomCenter, child: models[ind].Downloading ? new LinearProgressIndicator(value: models[ind].progress, backgroundColor:CColors.White, valueColor: new AlwaysStoppedAnimation<Unity.UIWidgets.ui.Color>(CColors.Blue)) : null)),
                                })
                            );
                            
                    });
            else
                return new SizedBox(width: 40, height: 40, child: new CircularProgressIndicator());
}

        private IPromise<object> showTextDialog()
        {
            return DialogUtils.showDialog(
                context: context,
                builder: (context) => 
                {
                    TextEditingController textEditingController = new TextEditingController();
                    textEditingController.text = "辛苦惹~";
                    return new SimpleDialog(
                    title: new Text("编辑文本内容"),
                    children: new List<Widget>
                    {
                        new TextField(
                            textInputAction: Unity.UIWidgets.service.TextInputAction.go,
                            controller: textEditingController
                        ),
                        new Row(
                            mainAxisAlignment: MainAxisAlignment.spaceAround,
                            children: new List<Widget>
                            {
                                new FlatButton(
                                    child: new Text("取消"),
                                    onPressed: () => {Navigator.of(context).pop(null);}
                                    ),
                                new FlatButton(
                                    child: new Text("确定"),
                                    onPressed: () => {Navigator.of(context).pop(textEditingController.text);}
                                    )
                            }
                        )
                    });
                });
        }

        private IPromise<object> showSelectDialog()
        {
            return DialogUtils.showDialog(
                context: context,
                builder: (context) =>
                {
                    TextEditingController textEditingController = new TextEditingController();
                    return new SimpleDialog(
                    title: new Text("选择来源"),
                    children: new List<Widget>
                    {
                        new SimpleDialogOption(
                            child: new Text("打开相机"),
                            onPressed: () => {Navigator.of(context).pop(false);}
                            ),
                        new SimpleDialogOption(
                            child: new Text("从相册中选择"),
                            onPressed: () => {Navigator.of(context).pop(true);}
                            ),
                    });
                });
        }

        private IPromise<object> showModelBottomSheet(BuildContext cont)
        {
            return BottomSheetUtils.showBottomSheet(
                context: cont,
                builder: (context) =>
                {
                    bottomSheetContext = context;
                    PageController pc = new PageController(Model.typeNames.Count);
                    return new Container(
                        color: CColors.White,
                        height: 270,
                        child: new Stack(
                            children: new List<Widget>
                            {
                                new Align(
                                    alignment: Alignment.topRight, 
                                    child: new Container(
                                        height: 20,
                                        width: 30,
                                        color: CColors.Transparent,
                                        child: new IconButton(
                                            color: CColors.Transparent,
                                            iconSize: 16,
                                            onPressed: () =>
                                            {
                                                Navigator.of(context).pop(null);
                                            },
                                            icon: new Icon(
                                                icon: MyIcons.close,
                                                color: CColors.Black
                                                )
                                            )
                                        )
                                    ),
                                new Align(alignment: Alignment.topCenter, child:
                                    new Column(
                                        children: new List<Widget>
                                        {
                                            new Row(
                                                children: new List<Widget>
                                                {
                                                    new GestureDetector(
                                                        onTap: () => 
                                                        {
                                                            if (widget.viewModel.modelIndex!=0)
                                                            {
                                                                widget.actionModel.ChangeModelIndex(0);
                                                                ((StatefulElement)context).markNeedsBuild();
                                                                modelsPc.jumpToPage(0);
                                                            }
                                                        },
                                                        child: new Container(
                                                            decoration: widget.viewModel.modelIndex == 0 ? new BoxDecoration(borderRadius: BorderRadius.all(10), color: CColors.Grey) : null,
                                                            padding: EdgeInsets.fromLTRB(5,5,5,5),
                                                            child: new Text(
                                                                style: new TextStyle(
                                                                    color: CColors.Black,
                                                                    fontSize: 16,
                                                                    fontWeight: widget.viewModel.modelIndex==0 ? FontWeight.w900 : FontWeight.w300
                                                                    ),
                                                                data: "全部"
                                                                )
                                                            )
                                                        ),
                                                    new GestureDetector(
                                                        onTap: () =>
                                                        {
                                                            if (widget.viewModel.modelIndex!=1)
                                                            {
                                                                widget.actionModel.ChangeModelIndex(1);
                                                                ((StatefulElement)context).markNeedsBuild();
                                                                modelsPc.jumpToPage(1);
                                                            }
                                                        },
                                                        child: new Container(
                                                            decoration: widget.viewModel.modelIndex == 1 ? new BoxDecoration(borderRadius: BorderRadius.all(10), color: CColors.Grey) : null,
                                                            padding: EdgeInsets.fromLTRB(5,5,5,5),
                                                            child: new Text(
                                                                style: new TextStyle(
                                                                    color: CColors.Black,
                                                                    fontSize: 16,
                                                                    fontWeight: widget.viewModel.modelIndex==1 ? FontWeight.w900 : FontWeight.w300
                                                                    ),
                                                                data: "场景"
                                                                )
                                                            )
                                                        ),
                                                    new GestureDetector(
                                                        onTap: () =>
                                                        {
                                                            if (widget.viewModel.modelIndex!=2)
                                                            {
                                                                widget.actionModel.ChangeModelIndex(2);
                                                                ((StatefulElement)context).markNeedsBuild();
                                                                modelsPc.jumpToPage(2);
                                                            }
                                                        },
                                                        child: new Container(
                                                            decoration: widget.viewModel.modelIndex == 2 ? new BoxDecoration(borderRadius: BorderRadius.all(10), color: CColors.Grey) : null,
                                                            padding: EdgeInsets.fromLTRB(5,5,5,5),
                                                            child: new Text(
                                                                style: new TextStyle(
                                                                    color: CColors.Black,
                                                                    fontSize: 16,
                                                                    fontWeight: widget.viewModel.modelIndex==2 ? FontWeight.w900 : FontWeight.w300
                                                                    ),
                                                                data: "人物"
                                                                )
                                                            )
                                                        ),
                                                }),
                                            new Expanded(flex: 1, child:
                                                new Container(
                                                    child: new PageView(
                                                        onPageChanged: value =>
                                                        {
                                                            widget.actionModel.ChangeModelIndex(value);
                                                            ((StatefulElement)context).markNeedsBuild();
                                                        },
                                                        controller: modelsPc,
                                                        children: new List<Widget>
                                                        {
                                                            new Container(
                                                                alignment: Alignment.center,
                                                                child: _modelsPreview(0, context)
                                                                ),
                                                            new Container(
                                                                alignment: Alignment.center,
                                                                child: _modelsPreview(1, context)
                                                                ),
                                                            new Container(
                                                                alignment: Alignment.center,
                                                                child: _modelsPreview(2, context)
                                                                ),
                                                        })
                                                    )
                                                ),
                                        })
                                    )
                            })
                        );
                })._completer;
        }

        private Widget runningCircle(int index)
        {
            if (!widget.viewModel.productShow) return null;
            if (widget.viewModel.products.Length <= index) return null;
            return new GestureDetector(
                onPanStart: detail =>
                {
                    setState(() =>
                    {
                        circleDragLeft = left[widget.viewModel.productIndex[index]];
                        circleDragBottom = bottom[widget.viewModel.productIndex[index]];
                        drag[index] = true;
                    });
                    Debug.Log("onPanStart");
                    Globals.instance.dragger.StartCreate(widget.viewModel.products[index]);
                },
                onPanEnd: detail =>
                {
                    setState(() =>
                    {
                        drag[index] = false;
                    });
                    Debug.Log("onPanEnd");
                    Globals.instance.dragger.StopCreate();
                },
                onPanUpdate: detail =>
                {
                    setState(() =>
                    {
                        circleDragLeft += detail.delta.dx;
                        circleDragBottom -= detail.delta.dy;
                    });
                },
                child: new AnimatedOpacity(
                    opacity: widget.viewModel.productAppear[widget.viewModel.productIndex[index]] ? 1 : 0,
                    duration: new System.TimeSpan(0,0,0,0,300),
                    child: new SizedBox(
                        //width: 46,
                        //height: 46,
                        child: new RaisedButton(
                            elevation: 2,
                            disabledElevation: 2,
                            shape: new CircleBorder(),
                            color: CColors.Transparent,
                            child: new IconButton(
                                icon: new Icon(
                                    color: CColors.White,
                                    size: 28,
                                    icon: widget.viewModel.allIcons[widget.viewModel.products[widget.viewModel.productIndex[index]].type.Get()]
                                    )
                                )
                            )
                        )
                    )
                );
        }

        private Widget stopCircle(int index)
        {
            return new Positioned(
                left: left[widget.viewModel.productIndex[index]],
                bottom: bottom[widget.viewModel.productIndex[index]],
                child: runningCircle(index)
                );
        }

        private Widget runningCirclePosition(int index)
        {
            return new AnimatedPositioned(
                curve: Curves.easeIn,
                duration: drag[index] ? new System.TimeSpan(0, 0, 0, 0, 0) : new System.TimeSpan(0, 0, 0, 0, 300),
                left: drag[index] ? circleDragLeft : widget.viewModel.productSpan ? left[widget.viewModel.productIndex[index]] : 0,
                bottom: drag[index] ? circleDragBottom : widget.viewModel.productSpan ? bottom[widget.viewModel.productIndex[index]] : 0,
                child: runningCircle(index)
                );
        }

        public Widget _buildMain()
        {
            return new Scaffold(
                backgroundColor: CColors.Transparent,
                resizeToAvoidBottomPadding: false, 
                bottomNavigationBar: new Builder(builder: context=> {
                    return new SizedBox(height: 79, child:
                    new BottomNavigationBar(
                        type: BottomNavigationBarType.fix,
                        elevation: 0,
                        backgroundColor: CColors.Transparent,
                        selectedItemColor: CColors.White,
                        unselectedItemColor: CColors.White,
                        selectedFontSize: 12,
                        unselectedFontSize: 12,
                        onTap: (value) =>
                        {
                            int nowLength = widget.viewModel.products.Length;
                            switch (value)
                            {
                                case 0:
                                {
                                    showTextDialog().Then((content) =>
                                    {
                                        string data = (string)content;
                                        if (data != null)
                                        {
                                            if (data.Length == 0)
                                                return;
                                            this.widget.actionModel.AddTextProductFuc(data);
                                        }
                                    });
                                break;
                                }
                                case 1:
                                {
                                    showSelectDialog().Then((content) =>
                                    {
                                        bool data = (bool)content;
                                        if (data)
                                        {
                                            NativeCall.OpenPhoto((Texture2D tex) =>
                                            {
                                                Texture2D source = tex;
                                                RenderTexture renderTex = RenderTexture.GetTemporary(
                                                    source.width,
                                                    source.height,
                                                    0,
                                                    RenderTextureFormat.Default,
                                                    RenderTextureReadWrite.Linear);

                                                Graphics.Blit(source, renderTex);
                                                RenderTexture previous = RenderTexture.active;
                                                RenderTexture.active = renderTex;
                                                Texture2D readableText = new Texture2D(source.width, source.height);
                                                readableText.ReadPixels(new UnityEngine.Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                                                readableText.Apply();
                                                RenderTexture.active = previous;
                                                RenderTexture.ReleaseTemporary(renderTex);
                                                
                                                widget.actionModel.AddImageProductFuc(readableText, context);

                                            });
                                        }
                                        else
                                        {
                                            NativeCall.OpenCamera((Texture2D tex) =>
                                            {
                                                Texture2D source = tex;
                                                RenderTexture renderTex = RenderTexture.GetTemporary(
                                                    source.width,
                                                    source.height,
                                                    0,
                                                    RenderTextureFormat.Default,
                                                    RenderTextureReadWrite.Linear);

                                                Graphics.Blit(source, renderTex);
                                                RenderTexture previous = RenderTexture.active;
                                                RenderTexture.active = renderTex;
                                                Texture2D readableText = new Texture2D(source.width, source.height);
                                                readableText.ReadPixels(new UnityEngine.Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                                                readableText.Apply();
                                                RenderTexture.active = previous;
                                                RenderTexture.ReleaseTemporary(renderTex);

                                                this.widget.actionModel.AddImageProductFuc(readableText, context);
                                            });
                                        }
                                    });
                                    break;
                                }
                                case 2:
                                    {

                                        showSelectDialog().Then((content) =>
                                        {
                                            bool data = (bool)content;
                                            if (data)
                                            {
                                                NativeCall.OpenVideo(path =>
                                                {
                                                    widget.actionModel.AddVideoProductFuc(path, context);
                                                });
                                            }
                                            else
                                            {
                                                NativeCall.OpenCameraVideo(path =>
                                                {
                                                    widget.actionModel.AddVideoProductFuc(path, context);
                                                });
                                            }
                                        });
                                        break;
                                    }
                                case 3:
                                {
                                        //setState(() => { bottomShow = true; });
                                        showModelBottomSheet(context);
                                        break;
                                }
                            }
                        },
                        items: new List<BottomNavigationBarItem>
                        {
                            new BottomNavigationBarItem(
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[0], size: 24, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "文字", style: new TextStyle(fontSize: 12)))
                                ),
                            new BottomNavigationBarItem(
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[1], size: 24, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "图片", style: new TextStyle(fontSize: 12)))
                                ),
                            new BottomNavigationBarItem(
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[2], size: 24, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "视频", style: new TextStyle(fontSize: 12)))
                                ),
                            new BottomNavigationBarItem(
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[3], size: 24, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "模型", style: new TextStyle(fontSize: 12)))
                                ),
                        }));
                    }),
                body: new Column(
                    verticalDirection: VerticalDirection.up,
                    children: new List<Widget>
                    {
                        new SizedBox(
                            height: 90,
                            child: moveContainer()
                        ),
                        new Padding(padding: EdgeInsets.only(bottom: 20)),
                        moveStack(),
                    })
                );
        }

        public Widget moveContainer()
        {
            return new GestureDetector(
                onPanEnd: detail =>
                {
                    if (detail.velocity.pixelsPerSecond.dx > 0)
                    {
                        if (widget.viewModel.cameraType < 2) widget.actionModel.ChangeCameraType(widget.viewModel.cameraType + 1);
                    }
                    else if (detail.velocity.pixelsPerSecond.dx < 0)
                    {
                        if (widget.viewModel.cameraType > 0) widget.actionModel.ChangeCameraType(widget.viewModel.cameraType - 1);
                    }
                },
                child: new Container(
                    color: CColors.Transparent,
                    child: new Stack(
                        children: new List<Widget>
                        {
                            new Align(
                                alignment: Alignment.topCenter,
                                child: new AnimatedPadding(
                                    duration: new System.TimeSpan(0,0,0,0,100),
                                    curve: Curves.easeInOut,
                                    padding: widget.viewModel.cameraType==0 ? EdgeInsets.only(left: -44, right: 44) : (widget.viewModel.cameraType == 1 ? EdgeInsets.zero : EdgeInsets.only(left:44, right: -44)),
                                    //padding: isFirst? EdgeInsets.fromLTRB(animation_first.value - 44, 0, 44 - animation_first.value, 0) : EdgeInsets.fromLTRB(animation_second.value - 44, 0, 44 - animation_second.value, 0),
                                    child: new Container(
                                        width: 220,
                                        padding: EdgeInsets.only(left: 44, right: 44),
                                        child: new Container(
                                            width: 132,
                                            height: 21,
                                            child: new Row(
                                                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                                                    children: new List<Widget>
                                                    {
                                                        new Listener(
                                                            onPointerDown: detail => {if(widget.viewModel.cameraType != 2) widget.actionModel.ChangeCameraType(2);},
                                                            child: new Text(data: "截取", style: widget.viewModel.cameraType==2 ? CTextStyle.strength: CTextStyle.normal)),
                                                        new Listener(
                                                            onPointerDown: detail => {if(widget.viewModel.cameraType != 1) widget.actionModel.ChangeCameraType(1);},
                                                            child: new Text(data: "录制", style: widget.viewModel.cameraType==1 ? CTextStyle.strength: CTextStyle.normal)),
                                                        new Listener(
                                                            onPointerDown: detail => {if(widget.viewModel.cameraType != 0) widget.actionModel.ChangeCameraType(0);},
                                                            child: new Text(data: "创作", style: widget.viewModel.cameraType==0 ? CTextStyle.strength: CTextStyle.normal)),
                                                    })
                                                )
                                            )
                                        )
                                    ),
                            new Align(
                                alignment: new Alignment(0, 1.0f),
                                child: new Listener(
                                    onPointerDown: detail=>
                                    {
                                        if(widget.viewModel.cameraType == 2)
                                        {
                                            Debug.Log("onrshot!");
                                            var oneShot = Camera.main.gameObject.AddComponent<OneShot>();
                                                oneShot.Shot(false, false, (texture) =>
                                                {
                                                    widget.actionModel.AddImageProductFuc(texture, context);
                                                });
                                        }
                                        else if(widget.viewModel.cameraType == 0)
                                        {
                                            Globals.instance.contextStack.Push(context);
                                            Navigator.push(context, new MaterialPageRoute(builder: (_) =>
                                            {
                                                return new StoreProvider<AppState>(
                                                    store: StoreProvider.store,
                                                    new MaterialApp(
                                                        home: new UploadScreenConnector()
                                                    )
                                                );
                                            }));
                                        }
                                    },
                                    child: new SizedBox(
                                        width: 60,
                                        height:60,
                                        child: new RaisedButton(
                                            shape: new CircleBorder(new BorderSide(color: CColors.White, width: 5)),
                                            color: CColors.Transparent,
                                            disabledColor: CColors.Transparent,
                                            elevation: 0,
                                            disabledElevation: 0,
                                            padding: widget.viewModel.cameraType == 0 ? EdgeInsets.only(right: 5) : null,
                                            child: widget.viewModel.cameraType == 0 ?
                                                    new Icon(
                                                        color: CColors.White,
                                                        size: 28,
                                                        icon: MyIcons.upload_mine
                                                    ) : (widget.viewModel.cameraType == 1 ?
                                                    new Icon(
                                                        color: CColors.FlatGreen,
                                                        size: 24,
                                                        icon: MyIcons.camera_button_mine
                                                    ) : null
                                                )
                                            )
                                        )
                                    )
                                ),
                        })
                    )
                );
        }

        public Widget moveStack()
        {
            return new Container(
                height: 800,
                child: new Stack(
                    children: new List<Widget>
                    {
                        new Align(
                            alignment: Alignment.bottomLeft,
                            child: new RaisedButton(
                                shape: new CircleBorder(),
                                disabledColor: CColors.Transparent,
                                disabledElevation: 2,
                                child: new IconButton(
                                    color: CColors.White,
                                    onPressed: () =>
                                    {
                                        if(widget.viewModel.products.Length == 0)
                                            return;
                                        if(!widget.viewModel.productSpan)
                                        {
                                            widget.actionModel.ChangeSpanState(true);
                                            widget.actionModel.ChangeShowState(true);
                                        }
                                        else
                                        {
                                            widget.actionModel.ChangeSpanState(false);
                                            Window.instance.startCoroutine(wait_300());
                                        }
                                    },
                                    icon: widget.viewModel.productSpan ? new Icon(
                                        size: 28,
                                        icon: MyIcons.delete_mine
                                        ) : new Icon(
                                        size: 28,
                                        icon: MyIcons.add_mine
                                        )
                                    )
                                )
                            ),
                        runningCirclePosition(0),
                        runningCirclePosition(1),
                        runningCirclePosition(2),
                    })
            );
        }

        public Widget _setting()
        {
            return new AnimatedContainer(
                decoration: new BoxDecoration(
                    borderRadius: BorderRadius.all(5.0f),
                    boxShadow: new List<BoxShadow> { new BoxShadow(color: CColors.Grey80, blurRadius: 1.0f)}
                    ),
                padding: EdgeInsets.only(left: 11, right: 11),
                //color: new Unity.UIWidgets.ui.Color(0xff343434),
                color: CColors.Grey80,
                width: 196,
                height: settingDown ? 340 : 0,
                duration: new System.TimeSpan(0, 0, 0, 0, 300),
                curve: Curves.easeInOut,
                child: new Column(
                    children: new List<Widget>
                    {
                        new Padding(
                            padding: EdgeInsets.only(left: 126, top: 5),
                            child: new IconButton(
                                onPressed:()=>{setState(()=>{settingDown = false; }); },
                                color: CColors.Blue,
                                iconSize: 36,
                                icon: new Icon(icon: MyIcons.settings, color: CColors.White)
                                )
                            ),
                        new Padding(padding: EdgeInsets.only(top: 8)),
                        new Divider(height: 1, color: CColors.White),
                        new Padding(padding: EdgeInsets.only(top: 11)),
                        new Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: new List<Widget>
                            {
                                new Text(
                                    data: "弹幕可见",
                                    style: new TextStyle(
                                        color: CColors.White,
                                        fontSize: 14,
                                        fontFamily: "Roboto-regular",
                                        fontWeight: FontWeight.w100,
                                        decoration: TextDecoration.none
                                        )
                                    ),
                                new SizedBox(
                                    //width: 44,
                                    height: 24,
                                    child: new CupertinoSwitch(
                                        value: true,
                                        onChanged: value=>{ })
                                    ),
                            }),
                        new Padding(padding: EdgeInsets.only(top: 16)),
                        new Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: new List<Widget>
                            {
                                new Text(
                                    data: "点云可见",
                                    style: new TextStyle(
                                        color: CColors.White,
                                        fontSize: 14,
                                        fontFamily: "Roboto-regular",
                                        fontWeight: FontWeight.w100,
                                        decoration: TextDecoration.none
                                        )
                                    ),
                                new SizedBox(
                                    //width: 44, 
                                    height: 24,
                                    child: new CupertinoSwitch(
                                        value: true,
                                        onChanged: value=>{ })
                                    ),
                            }),
                        new Padding(padding: EdgeInsets.only(top: 16)),
                        new Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: new List<Widget>
                            {
                                new Text(
                                    data: "视频播放",
                                    style: new TextStyle(
                                        color: CColors.White,
                                        fontSize: 14,
                                        fontFamily: "Roboto-regular",
                                        fontWeight: FontWeight.w100,
                                        decoration: TextDecoration.none
                                        )
                                    ),
                                new SizedBox(
                                    //width: 44,
                                    height: 24,
                                    child: new CupertinoSwitch(
                                        value: true,
                                        onChanged: value=>{ })
                                    ),
                            }),
                        new Padding(padding: EdgeInsets.only(top: 16)),
                        new Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: new List<Widget>
                            {
                                new Text(
                                    data: "自由移动",
                                    style: new TextStyle(
                                        color: CColors.White,
                                        fontSize: 14,
                                        fontFamily: "Roboto-regular",
                                        fontWeight: FontWeight.w100,
                                        decoration: TextDecoration.none
                                        )
                                    ),
                                new SizedBox(
                                    //width: 44,
                                    height: 24,
                                    child: new CupertinoSwitch(
                                        value: true,
                                        onChanged: value=>{ })
                                    ),
                            }),
                        new Padding(padding: EdgeInsets.only(top: 21)),
                        new Divider(height: 1, color: CColors.White),
                        new Padding(padding: EdgeInsets.only(top: 7)),
                        new Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: new List<Widget>
                            {
                                new Text(
                                    data: "自动保存地图",
                                    style: new TextStyle(
                                        color: CColors.White,
                                        fontSize: 14,
                                        fontFamily: "Roboto-regular",
                                        fontWeight: FontWeight.w100,
                                        decoration: TextDecoration.none
                                        )
                                    ),
                                new SizedBox(
                                   //width: 44,
                                    height: 24,
                                    child: new CupertinoSwitch(
                                        value: true,
                                        onChanged: value=>{ })
                                    ),
                            }),
                        new Padding(padding: EdgeInsets.only(top: 16)),
                        new Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: new List<Widget>
                            {
                                new Text(
                                    data: "点云数量",
                                    style: new TextStyle(
                                        color: CColors.White,
                                        fontSize: 14,
                                        fontFamily: "Roboto-regular",
                                        fontWeight: FontWeight.w100,
                                        decoration: TextDecoration.none
                                        )
                                    ),
                                new Text(
                                    data: "1000",
                                    style: new TextStyle(
                                        color: CColors.White,
                                        fontSize: 12,
                                        fontFamily: "Roboto-regular",
                                        fontWeight: FontWeight.w100,
                                        decoration: TextDecoration.none
                                        )
                                    ),
                            }),
                        new Padding(padding: EdgeInsets.only(top: 7)),
                        new Slider(value: pointCloudProgress, onChanged:value =>{setState(()=>{ pointCloudProgress=value;}); }, activeColor: CColors.Blue, inactiveColor: CColors.Black),
                    })
                );
        }
        public override Widget build(BuildContext context)
        {
            return new Container(
                color: CColors.Transparent,
                child: new Stack(
                    fit: StackFit.loose,
                    children: new List<Widget>
                    {
                        
                        new Align(alignment: Alignment.bottomCenter, child: _buildMain()),
                        new Positioned(
                            top: 8,
                            left: 11,
                            child: new IconButton(
                                color: CColors.Transparent,
                                iconSize: 24,
                                onPressed: () =>
                                {
                                    Debug.Log("p");
                                    BuildContext lastContext = Globals.instance.contextStack.Pop();
                                    Navigator.pop(lastContext);
                                    Globals.instance.map.SetActive(true);
                                    Globals.instance.arEffect.GetComponent<AREffectManager>().CreateAndEditMapEnd();
                                },
                                icon: new Icon(
                                    icon: MyIcons.cancel_mine,
                                    color: CColors.White
                                    )
                                )
                            ),
                        new Positioned(
                            top: 5,
                            right: 23,
                            child: new IconButton(
                                onPressed:()=>{setState(()=>{settingDown = true; }); },
                                color: CColors.Transparent,
                                iconSize: 36,
                                icon: new Icon(icon: MyIcons.settings, color: CColors.White)
                                )
                            ),
                        new Positioned(
                            top: 50, 
                            right: 23, 
                            child: new Listener(
                                onPointerUp:detail=>{Debug.Log("up"); },
                                child: new IconButton(
                                    onPressed:()=>{Debug.Log("delete!"); }, 
                                    color: CColors.Transparent,
                                    iconSize: 36, 
                                    icon: new Icon(icon: MyIcons.delete, color: CColors.White)))),
                        new Positioned(
                            top: 0,
                            right: 12,
                            child: _setting()
                            ),
                    })
                );
        }
    }

}
