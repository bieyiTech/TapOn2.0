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
                        products = state.settingState.products,
                        allIcons = state.settingState.allIcons,
                        objects = state.settingState.objects,
                    };
                },
                builder: (context1, viewModel, dispatcher) =>
                {
                    var actionModel = new SettingScreenActionModel
                    {
                        ChangeIndex = (value) =>
                        { dispatcher.dispatch(new ChangeIndexAction() { index = value, }); },
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

        //TabController _tabController;
        //ScrollController _scrollController;
        Animation<int> animation_first;
        Animation<int> animation_second;
        AnimationController animationController_first;
        AnimationController animationController_second;

        bool completeReset = false;
        bool dismissReset = false;
        bool isFirst = true;

        //int nowPadding = 50;
        int cameraType = 0;

        List<float> left = new List<float> { 0, 56.6f, 80 };
        List<float> bottom = new List<float> { 80, 56.6f, 0 };
        bool span = false;
        bool show = false;
        //readonly List<float> data = new List<float> { 0, 80, 56.6f, 56.6f, 80, 0 };

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

        private IEnumerator wait_300()
        {
            yield return new UIWidgetsWaitForSeconds(0.3f);
            setState(() => { show = span; });
        }

        public Widget _buildBottom()
        {
            return new Scaffold(
                backgroundColor: CColors.Transparent,
                floatingActionButtonLocation: FloatingActionButtonLocation.centerFloat,
                floatingActionButtonAnimator: FloatingActionButtonAnimator.scaling,
                floatingActionButton: new FloatingActionButton(
                    elevation: 0,
                    shape: new CircleBorder(new BorderSide(color: CColors.White, width: 3)),
                    backgroundColor: CColors.Transparent,
                    child: new IconButton(
                            icon: new Icon(
                                color: CColors.White,
                                //size: 30,
                                icon: MyIcons.upload
                            )
                        )
                    ),
                bottomNavigationBar: new BottomNavigationBar(
                    type: BottomNavigationBarType.fix,
                    selectedItemColor: CColors.Black,
                    unselectedItemColor: CColors.Black,
                    onTap: (value) =>
                    {
                        switch (value)
                        {
                            case 0:
                                {
                                    showTextDialog().Then((content) =>
                                    {
                                        string data = (string)content;
                                        if (data != null)
                                        {
                                            this.widget.actionModel.ChangeIndex(0);
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
                                            this.widget.actionModel.ChangeIndex(1);
                                        else
                                            this.widget.actionModel.ChangeIndex(11);
                                    });
                                    break;
                                }
                        }
                    },
                    items: new List<BottomNavigationBarItem>
                    {
                        new BottomNavigationBarItem(
                                icon: new Icon(icon: this.widget.viewModel.allIcons[0], size: 30, color: CColors.Black),
                                title: new Text("文字")
                            ),
                        new BottomNavigationBarItem(
                                icon: new Icon(icon: this.widget.viewModel.allIcons[1], size: 30, color: CColors.Black),
                                title: new Text("图片")
                            ),
                        new BottomNavigationBarItem(
                                icon: new Icon(icon: this.widget.viewModel.allIcons[2], size: 30, color: CColors.Black),
                                title: new Text("视频")
                            ),
                        new BottomNavigationBarItem(
                                icon: new Icon(icon: this.widget.viewModel.allIcons[3], size: 30, color: CColors.Black),
                                title: new Text("模型")
                            ),
                    }
                    ),
                body: new GestureDetector(
                        onPanEnd: detail =>
                        {
                            if (detail.velocity.pixelsPerSecond.dx < 0)
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
                            else if(detail.velocity.pixelsPerSecond.dx > 0)
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
                                            )
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
                        height: 400,
                        child:new Stack(
                            children: new List<Widget>
                            {
                                /*new Align(
                                    alignment: Alignment.center,
                                    child: new IconButton(
                                        onPressed: () =>
                                        {
                                            Navigator.pop(Prefabs.homeContext);
                                        },
                                        icon: new Icon(
                                            icon: MyIcons.tab_home_fill,
                                            color: CColors.Black
                                            )
                                        )
                                    ),*/
                                new Align(
                                    alignment: Alignment.bottomLeft,
                                    child: new RaisedButton(
                                        shape: new CircleBorder(),
                                        color: CColors.Transparent,
                                        child: new IconButton(
                                            onPressed: () =>
                                            {
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
                                                icon: MyIcons.delete
                                                ) : new Icon(
                                                size: 28,
                                                icon: MyIcons.add
                                                )
                                            )
                                        )
                                    ),
                                new AnimatedPositioned(
                                    curve: Curves.easeIn,
                                    duration: new System.TimeSpan(0,0,0,0,300),
                                    left: span ? left[0] : 0,
                                    bottom: span ? bottom[0] : 0,
                                    child: show ? new RaisedButton(
                                        shape: new CircleBorder(),
                                        color: CColors.Transparent,
                                        child: new IconButton(
                                            icon: new Icon(
                                                size: 28,
                                                icon: MyIcons.add
                                                )
                                            )
                                        ) : null
                                    ),
                                new AnimatedPositioned(
                                    curve: Curves.easeIn,
                                    duration: new System.TimeSpan(0,0,0,0,300),
                                    left: span ? left[1] : 0,
                                    bottom: span ? bottom[1] : 0,
                                    child: show ? new RaisedButton(
                                        shape: new CircleBorder(),
                                        color: CColors.Transparent,
                                        child: new IconButton(
                                            icon: new Icon(
                                                size: 28,
                                                icon: MyIcons.add
                                                )
                                            )
                                        ) : null
                                    ),
                                new AnimatedPositioned(
                                    curve: Curves.easeIn,
                                    duration: new System.TimeSpan(0,0,0,0,300),
                                    left: span ? left[2] : 0,
                                    bottom: span ? bottom[2] : 0,
                                    child: show ? new RaisedButton(
                                        shape: new CircleBorder(),
                                        color: CColors.Transparent,
                                        child: new IconButton(
                                            icon: new Icon(
                                                size: 28,
                                                icon: MyIcons.add
                                                )
                                            )
                                        ) : null
                                    ),
                            })
                    ),
                }
                );
        }

        public override Widget build(BuildContext context)
        {
            return new Container(
                color: CColors.Transparent,
                child: new Stack(
                    children: new List<Widget>
                    {
                        new Align(
                                    alignment: Alignment.topLeft,
                                    child: new IconButton(
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
