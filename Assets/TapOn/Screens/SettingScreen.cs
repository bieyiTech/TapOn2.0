﻿using RSG;
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
                        objects = state.settingState.objects,
                        models = state.settingState.models,
                    };
                },
                builder: (context1, viewModel, dispatcher) =>
                {
                    var actionModel = new SettingScreenActionModel
                    {
                        ChangeIndex = (value) =>
                        { dispatcher.dispatch(new ChangeIndexAction() { index = value, }); },
                        AddTextProduct = (value) =>
                        { dispatcher.dispatch(new AddTextProductAction() { text = value, }); },
                        AddImageProduct = (value) =>
                        { dispatcher.dispatch(new AddImageProductAction() { texture = value, }); },
                        SetModelsMessage = (models) =>
                        { dispatcher.dispatch(new SetModelsMessageAction() { models = models, }); },
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
        //TabController _tabController;
        //ScrollController _scrollController;
        Animation<int> animation_first;
        Animation<int> animation_second;
        AnimationController animationController_first;
        AnimationController animationController_second;

        bool completeReset = false;
        bool dismissReset = false;
        bool isFirst = true;

        int cameraType = 0;

        List<float> left = new List<float> { 0, 56.6f, 80 };
        List<float> bottom = new List<float> { 80, 56.6f, 0 };
        bool span = true;
        bool show = true;

        List<bool> drag = new List<bool> {false, false, false };
        float circleDragLeft = 0;
        float circleDragBottom = 0;

        /// <summary>
        /// 转盘移动的参数
        /// </summary>
        List<int> productIndex = new List<int> {0,1,2 };
        List<bool> appear = new List<bool> { false, false, false };
        bool moveByCircle = false;
        List<bool> moveStep = new List<bool> { false, false, false, false, false };
        List<List<float>> circleLeft = new List<List<float>> { new List<float> { 56.6f, 47.0f, 36.3f, 24.7f, 12.5f, 0 }, new List<float> { 80, 79.0f, 76.1f, 71.3f, 64.7f, 56.6f } };
        List<List<float>> circleBottom = new List<List<float>> { new List<float> { 56.6f, 64.7f, 71.3f, 76.1f, 79.0f, 80 }, new List<float> { 0, 12.5f, 24.7f, 36.3f, 47.0f, 56.6f } };

        List<Animation<float>> am_0_left = new List<Animation<float>>();
        List<Animation<float>> am_1_left = new List<Animation<float>>();
        List<Animation<float>> am_0_bottom = new List<Animation<float>>();
        List<Animation<float>> am_1_bottom = new List<Animation<float>>();
        List<AnimationController> ac_0_left = new List<AnimationController>();
        List<AnimationController> ac_1_left = new List<AnimationController>();
        List<AnimationController> ac_0_bottom = new List<AnimationController>();
        List<AnimationController> ac_1_bottom = new List<AnimationController>();

        PageController modelsPc = new PageController(initialPage: 0);

        int modelIndex = 0;

        bool modelsMessageReady = false;
        bool modelsPreviewReady = false;

        BuildContext bottomSheetContext;

        public override void initState()
        {
            base.initState();
            animationController_first = new AnimationController(vsync: this, duration: new System.TimeSpan(0,0,0,0,500));
            animationController_second = new AnimationController(vsync: this, duration: new System.TimeSpan(0,0,0,0,500));
            //animation = new CurveTween(new CurvedAnimation()).animate(animationController);
            
            animation_first = new StepTween(0, 50).chain(new CurveTween(Curves.easeIn)).animate(animationController_first);
            animation_second = new StepTween(50, 100).chain(new CurveTween(Curves.easeIn)).animate(animationController_second);
            animation_first.addListener(() =>
            {
                setState(() => { });
            });
            animation_second.addListener(() =>
            {
                setState(() => { });
            });
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
                        models.Add(new Model
                        {
                            modelName = model.modelName,
                            modelType = model.modelType.Get(),
                            previewFileName = model.preview.filename,
                            previewUrl = model.preview.url,
                        });
                    }
                    this.widget.actionModel.SetModelsMessage(models);
                    modelsMessageReady = true;
                    if (bottomSheetContext != null)
                        ((StatefulElement)bottomSheetContext).markNeedsBuild();
                    break;
                }
            }
            
        }

        private void changeIndex()
        {
            int temp = productIndex[2];
            productIndex[2] = productIndex[1];
            productIndex[1] = productIndex[0];
            productIndex[0] = temp;
        }

        private IEnumerator waitForShow()
        {
            yield return new UIWidgetsWaitForSeconds(0.3f);
            setState(() => { appear[2] = true; });
        }

        private IEnumerator waitForMiss()
        {
            setState(() => { appear[2] = false; });
            yield return new UIWidgetsWaitForSeconds(0.3f);
            #region
            /*foreach (AnimationController ac in ac_0_left)
                ac.dispose();
            foreach (AnimationController ac in ac_1_left)
                ac.dispose();
            foreach (AnimationController ac in ac_0_bottom)
                ac.dispose();
            foreach (AnimationController ac in ac_1_bottom)
                ac.dispose();
            ac_0_left = new List<AnimationController>();
            ac_1_left = new List<AnimationController>();
            ac_0_bottom = new List<AnimationController>();
            ac_1_bottom = new List<AnimationController>();
            am_0_left = new List<Animation<float>>();
            am_1_left = new List<Animation<float>>();
            am_0_bottom = new List<Animation<float>>();
            am_1_bottom = new List<Animation<float>>();
            for(int i = 0; i < 5; i++)
            {
                ac_0_left.Add(new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, 0, 0, 60)));
                ac_1_left.Add(new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, 0, 0, 60)));
                ac_0_bottom.Add(new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, 0, 0, 60)));
                ac_1_bottom.Add(new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, 0, 0, 60)));
                am_0_left.Add(new FloatTween(circleLeft[0][i], circleLeft[0][i + 1]).animate(ac_0_left[i]));
                am_1_left.Add(new FloatTween(circleLeft[1][i], circleLeft[1][i + 1]).animate(ac_1_left[i]));
                am_0_bottom.Add(new FloatTween(circleBottom[0][i], circleBottom[0][i + 1]).animate(ac_0_bottom[i]));
                am_1_bottom.Add(new FloatTween(circleBottom[1][i], circleBottom[1][i + 1]).animate(ac_1_bottom[i]));
                am_0_left[i].addListener(() => { setState(() => { }); });
                am_1_left[i].addListener(() => { setState(() => { }); });
                am_0_bottom[i].addListener(() => { setState(() => { }); });
                am_1_bottom[i].addListener(() => { setState(() => { }); });
            }
            am_0_left[0].addStatusListener(status =>
            {
                if (status == AnimationStatus.completed)
                {
                    setState(() => { moveStep[0] = false; moveStep[1] = true; });
                    ac_0_left[1].forward();
                    ac_1_left[1].forward();
                    ac_0_bottom[1].forward();
                    ac_1_bottom[1].forward();
                }
            });
            am_0_left[1].addStatusListener(status =>
            {
                if (status == AnimationStatus.completed)
                {
                    setState(() => { moveStep[1] = false; moveStep[2] = true; });
                    ac_0_left[2].forward();
                    ac_1_left[2].forward();
                    ac_0_bottom[2].forward();
                    ac_1_bottom[2].forward();
                }
            });
            am_0_left[2].addStatusListener(status =>
            {
                if (status == AnimationStatus.completed)
                {
                    setState(() => { moveStep[2] = false; moveStep[3] = true; });
                    ac_0_left[3].forward();
                    ac_1_left[3].forward();
                    ac_0_bottom[3].forward();
                    ac_1_bottom[3].forward();
                }
            });
            am_0_left[3].addStatusListener(status =>
            {
                if (status == AnimationStatus.completed)
                {
                    setState(() => { moveStep[3] = false; moveStep[4] = true; });
                    ac_0_left[4].forward();
                    ac_1_left[4].forward();
                    ac_0_bottom[4].forward();
                    ac_1_bottom[4].forward();
                }
            });
            am_0_left[4].addStatusListener(status =>
            {
                if (status == AnimationStatus.completed)
                {
                    setState(() => { moveStep[4] = false; moveByCircle = false; });
                }
            });
            setState(() => { moveByCircle = true; });
            ac_0_left[0].forward();
            ac_1_left[0].forward();
            ac_0_bottom[0].forward();
            ac_1_bottom[0].forward();*/
            #endregion
            Window.instance.startCoroutine(waitForShow());
        }

        private void updateCircle()
        {
            setState(() => { changeIndex(); });
            Window.instance.startCoroutine(waitForMiss());
        }

        private List<Widget> _products()
        {
            List<Widget> all = new List<Widget>();
            foreach(Product product in this.widget.viewModel.products)
            {
                all.Add(
                    new IconButton(
                        icon: new Icon(
                            icon: this.widget.viewModel.allIcons[(int)product.type]
                            )
                        )
                    );
            }
            return all;
        }

        private Widget _modelsPreview(int index)
        {
            List<Model> models = new List<Model>();
            if (index == 0)
                models = widget.viewModel.models;
            else
            {
                foreach(Model m in widget.viewModel.models)
                {
                    if (m.modelType == index)
                        models.Add(m);
                }
            }
            if (modelsMessageReady)
                return GridView.builder(
                    gridDelegate: new SliverGridDelegateWithFixedCrossAxisCount(
                        crossAxisCount: 4,
                        childAspectRatio: 1.0f
                        ),
                    itemCount: models.Count,
                    itemBuilder: (context_grid, ind) =>
                    {
                        return new GestureDetector(
                            child: new Unity.UIWidgets.widgets.Image(
                                image: new NetworkImage(url: models[ind].previewUrl)));
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
                                                            if (modelIndex!=0)
                                                            {
                                                                modelIndex = 0;
                                                                ((StatefulElement)context).markNeedsBuild();
                                                                modelsPc.jumpToPage(0);
                                                            }
                                                        },
                                                        child: new Container(
                                                            decoration: modelIndex == 0 ? new BoxDecoration(borderRadius: BorderRadius.all(10), color: CColors.Grey) : null,
                                                            padding: EdgeInsets.fromLTRB(5,5,5,5),
                                                            child: new Text(
                                                                style: new TextStyle(
                                                                    color: CColors.Black,
                                                                    fontSize: 16,
                                                                    fontWeight: modelIndex==0 ? FontWeight.w900 : FontWeight.w300
                                                                    ),
                                                                data: "全部"
                                                                )
                                                            )
                                                        ),
                                                    new GestureDetector(
                                                        onTap: () =>
                                                        {
                                                            if (modelIndex!=1)
                                                            {
                                                                modelIndex = 1;
                                                                ((StatefulElement)context).markNeedsBuild();
                                                                modelsPc.jumpToPage(1);
                                                            }
                                                        },
                                                        child: new Container(
                                                            decoration: modelIndex == 1 ? new BoxDecoration(borderRadius: BorderRadius.all(10), color: CColors.Grey) : null,
                                                            padding: EdgeInsets.fromLTRB(5,5,5,5),
                                                            child: new Text(
                                                                style: new TextStyle(
                                                                    color: CColors.Black,
                                                                    fontSize: 16,
                                                                    fontWeight: modelIndex==1 ? FontWeight.w900 : FontWeight.w300
                                                                    ),
                                                                data: "场景"
                                                                )
                                                            )
                                                        ),
                                                    new GestureDetector(
                                                        onTap: () =>
                                                        {
                                                            if (modelIndex!=2)
                                                            {
                                                                modelIndex = 2;
                                                                ((StatefulElement)context).markNeedsBuild();
                                                                modelsPc.jumpToPage(2);
                                                            }
                                                        },
                                                        child: new Container(
                                                            decoration: modelIndex == 2 ? new BoxDecoration(borderRadius: BorderRadius.all(10), color: CColors.Grey) : null,
                                                            padding: EdgeInsets.fromLTRB(5,5,5,5),
                                                            child: new Text(
                                                                style: new TextStyle(
                                                                    color: CColors.Black,
                                                                    fontSize: 16,
                                                                    fontWeight: modelIndex==2 ? FontWeight.w900 : FontWeight.w300
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
                                                            modelIndex = value;
                                                            ((StatefulElement)context).markNeedsBuild();
                                                        },
                                                        controller: modelsPc,
                                                        children: new List<Widget>
                                                        {
                                                            new Container(
                                                                alignment: Alignment.center,
                                                                child: _modelsPreview(0)
                                                                ),
                                                            new Container(
                                                                alignment: Alignment.center,
                                                                child: _modelsPreview(1)
                                                                ),
                                                            new Container(
                                                                alignment: Alignment.center,
                                                                child: _modelsPreview(2)
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

        private IEnumerator wait_300()
        {
            yield return new UIWidgetsWaitForSeconds(0.3f);
            setState(() => { show = span; });
        }
        private IEnumerator wait_100_opacity(int index)
        {
            yield return new UIWidgetsWaitForSeconds(0.1f);
            setState(() => { appear[index] = true; });
        }

        private Widget runningCircle(int index)
        {
            if (!show) return null;
            if (widget.viewModel.products.Length <= index) return null;
            return new GestureDetector(
                onPanStart: detail =>
                {
                    setState(() =>
                    {
                        circleDragLeft = left[productIndex[index]];
                        circleDragBottom = bottom[productIndex[index]];
                        drag[index] = true;
                    });
                },
                onPanEnd: detail =>
                {
                    setState(() =>
                    {
                        drag[index] = false;
                    });
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
                    opacity: appear[productIndex[index]] ? 1 : 0,
                    duration: new System.TimeSpan(0,0,0,0,300),
                    child: new RaisedButton(
                        shape: new CircleBorder(),
                        color: CColors.Transparent,
                        child: new IconButton(
                            icon: new Icon(
                                size: 28,
                                icon: widget.viewModel.allIcons[(int)widget.viewModel.products[productIndex[index]].type]
                                )
                            )
                        )
                    )
                );
        }

        private Widget stopCircle(int index)
        {
            return new Positioned(
                left: left[productIndex[index]],
                bottom: bottom[productIndex[index]],
                child: runningCircle(index)
                );
        }

        private Widget runningCirclePosition(int index)
        {
            return new AnimatedPositioned(
                curve: Curves.easeIn,
                duration: drag[index] ? new System.TimeSpan(0, 0, 0, 0, 0) : new System.TimeSpan(0, 0, 0, 0, 300),
                left: drag[index] ? circleDragLeft : span ? left[productIndex[index]] : 0,
                bottom: drag[index] ? circleDragBottom : span ? bottom[productIndex[index]] : 0,
                child: runningCircle(index)
                );
        }

        public Widget _buildBottom()
        {
            return new Scaffold(
                backgroundColor: CColors.Transparent,
                bottomNavigationBar: new Builder(builder: context=> {
                    return new BottomNavigationBar(
                        type: BottomNavigationBarType.fix,
                        elevation: 0,
                        backgroundColor: CColors.Transparent,
                        selectedItemColor: CColors.White,
                        unselectedItemColor: CColors.White,
                        selectedFontSize: 14,
                        unselectedFontSize: 14,
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
                                            this.widget.actionModel.AddTextProduct(data);
                                            //this.widget.actionModel.ChangeIndex(0);
                                            if (nowLength < 3)
                                                Window.instance.startCoroutine(wait_100_opacity(nowLength));
                                            else
                                                updateCircle();
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
                                                widget.actionModel.AddImageProduct(tex);
                                            });
                                        }
                                        else
                                        {
                                            NativeCall.OpenCamera((Texture2D tex) =>
                                            {
                                                widget.actionModel.AddImageProduct(tex);
                                            });
                                        }
                                        if (nowLength < 3)
                                            Window.instance.startCoroutine(wait_100_opacity(nowLength));
                                        else
                                            updateCircle();
                                    });
                                    break;
                                }
                                case 2:
                                    {

                                        //to do after
                                        showUploadDialog().Then(message => { });
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
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[0], size: 30, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "文字", style: new TextStyle(fontSize: 14)))
                                ),
                            new BottomNavigationBarItem(
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[1], size: 30, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "图片", style: new TextStyle(fontSize: 14)))
                                ),
                            new BottomNavigationBarItem(
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[2], size: 30, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "视频", style: new TextStyle(fontSize: 14)))
                                ),
                            new BottomNavigationBarItem(
                                    icon: new Icon(icon: this.widget.viewModel.allIcons[3], size: 30, color: CColors.White),
                                    title: new Padding(padding: EdgeInsets.only(top: 5), child: new Text(data: "模型", style: new TextStyle(fontSize: 14)))
                                ),
                        });
                    }),
                body: new GestureDetector(
                        onPanEnd: detail =>
                        {
                            if (detail.velocity.pixelsPerSecond.dx > 0)
                            {
                                if (cameraType == 0)
                                {
                                    isFirst = true;
                                    cameraType++;
                                    animationController_first.forward();
                                }
                                else if(cameraType == 1)
                                {
                                    isFirst = false;
                                    cameraType++;
                                    animationController_second.forward();
                                }
                            }
                            else if(detail.velocity.pixelsPerSecond.dx < 0)
                            {
                                if(cameraType == 1)
                                {
                                    isFirst = true;
                                    cameraType--;
                                    animationController_first.reverse();
                                }
                                else if (cameraType == 2)
                                {
                                    isFirst = false;
                                    cameraType--;
                                    animationController_second.reverse();
                                }
                            }
                        },
                        onTap: () => { Debug.Log("ontap"); },
                        child: new Container(
                            color: CColors.Transparent,
                            child: new Stack(
                                children: new List<Widget>
                                {
                                    new Align(
                                        alignment: Alignment.topCenter,
                                        child: new Container(
                                            padding: isFirst? EdgeInsets.fromLTRB(animation_first.value - 50, 0, 50 - animation_first.value, 0) : EdgeInsets.fromLTRB(animation_second.value - 50, 0, 50 - animation_second.value, 0),
                                            child: new Container(
                                                    child: new Container(
                                                        width: 150,
                                                        height: 50,
                                                        child: new Row(
                                                                mainAxisAlignment: MainAxisAlignment.spaceAround,
                                                                children: new List<Widget>
                                                                {
                                                                    new Text(data: "截取", style: cameraType==2 ? CTextStyle.strength: CTextStyle.normal),
                                                                    new Text(data: "录制", style: cameraType==1 ? CTextStyle.strength: CTextStyle.normal),
                                                                    new Text(data: "创作", style: cameraType==0 ? CTextStyle.strength: CTextStyle.normal),
                                                                })
                                                            )
                                                    )
                                                )
                                            ),
                                    new Align(
                                        alignment: new Alignment(0, 0.5f),

                                        child: new Listener(
                                            onPointerDown: detail=>
                                            {
                                                Debug.Log("onpointerdown");
                                                Navigator.push(context, new MaterialPageRoute(builder: (_) =>
                                                    {
                                                        return new StoreProvider<AppState>(
                                                            store: StoreProvider.store,
                                                            new MaterialApp(
                                                                home: new UploadScreenConnector()
                                                            )
                                                        );
                                                    }));
                                            },
                                            child: new SizedBox(
                                                width: 72, height:72, child:
                                            new RaisedButton(
                                                shape: new CircleBorder(new BorderSide(color: CColors.White, width: 8)),
                                                elevation: 0,
                                                color: CColors.Transparent,
                                                disabledColor: CColors.Transparent,
                                                child: cameraType == 0 ? 
                                                        new Icon(
                                                            color: CColors.White,
                                                            size: 36,
                                                            icon: MyIcons.upload
                                                        ) : (cameraType == 1 ? 
                                                        new Icon(
                                                            color: CColors.WeChatGreen,
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
                        )
                );
        }

        public Widget _buildMain()
        {
            return new Column(
                //mainAxisSize: MainAxisSize.max,
                verticalDirection: VerticalDirection.up,
                children: new List<Widget>
                {
                    new Container(
                        height: 200,
                        child: _buildBottom()
                        ),
                    new Container(
                        height: 800,
                        child:new Stack(
                            children: new List<Widget>
                            {
                                new Align(
                                    alignment: Alignment.bottomLeft,
                                    child: new RaisedButton(
                                        shape: new CircleBorder(),
                                        color: CColors.Transparent,
                                        child: new IconButton(
                                            onPressed: () =>
                                            {
                                                if(widget.viewModel.products.Length == 0)
                                                    return;
                                                if(span == false)
                                                    setState(()=>{span = true; show = true; });
                                                else
                                                {
                                                    setState(()=>{span = !span; });
                                                    Window.instance.startCoroutine(wait_300());
                                                }
                                            },
                                            icon: span ? new Icon(
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
                    ),
                }
                );
        }

        public override Widget build(BuildContext context)
        {
            return new Container(
                color: CColors.Grey80,
                child: new Stack(
                    fit: StackFit.loose,
                    children: new List<Widget>
                    {
                        new Align(
                            alignment: Alignment.topLeft,
                            child: new IconButton(
                                color: CColors.Transparent,
                                iconSize: 24,
                                onPressed: () =>
                                {
                                    Navigator.pop(Prefabs.instance.homeContext);
                                    Prefabs.instance.map.SetActive(true);
                                },
                                icon: new Icon(
                                    icon: MyIcons.arrow_back,
                                    color: CColors.Black
                                    )
                                )
                            ),
                        new Align(alignment: Alignment.bottomCenter, child: _buildMain()),
                    }
                    )
                );
        }
    }

}
