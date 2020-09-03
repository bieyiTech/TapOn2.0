using System.Collections.Generic;
using TapOn.Constants;
using TapOn.Models.ActionModels;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using TapOn.Redux;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.widgets;

namespace TapOn.Screens
{
    public class FindScreenConnector : StatelessWidget
    {
        public FindScreenConnector(
            Key key = null
        ) : base(key: key)
        {
        }
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, FindScreenViewModel>(
                converter: state =>
                {
                    return new FindScreenViewModel
                    {

                    };
                },
                builder: (context1, viewModel, dispatcher) =>
                {
                    var actionModel = new FindScreenActionModel
                    {

                    };
                    return new FindScreen(viewModel: viewModel, actionModel: actionModel);
                }
            );
        }
    }
    public class FindScreen : StatefulWidget
    {
        public FindScreen(
        FindScreenViewModel viewModel = null,
        FindScreenActionModel actionModel = null,
        Key key = null
            ) : base(key: key)
        {
            this.viewModel = viewModel;
            this.actionModel = actionModel;
        }

        public readonly FindScreenViewModel viewModel;
        public readonly FindScreenActionModel actionModel;
        public override State createState()
        {
            _FindScreenState s = new _FindScreenState();
            return s;
        }
    }

    public class _FindScreenState : State<FindScreen>
    {
        bool preview = false;

        bool camera_pic = true;

        private Widget imagePreview()
        {
            if (preview)
                return new Container(width: 300, height: 300, color: CColors.Blue);
            return new Container();
        }

        public override Widget build(BuildContext context)
        {
            return new Container(
                color: CColors.Transparent,
                child: new Unity.UIWidgets.widgets.Stack(
                    children: new List<Widget>
                    {
                        new Align(
                            alignment: Alignment.bottomCenter,
                            child: new GestureDetector(
                                onPanEnd: detail =>
                                {
                                    if (detail.velocity.pixelsPerSecond.dx > 0 && camera_pic)
                                        setState(()=>{camera_pic = false; });
                                    if (detail.velocity.pixelsPerSecond.dx < 0 && !camera_pic)
                                        setState(()=>{camera_pic = true; });
                                },
                                child: new Container(
                                    width: 600,
                                    color: CColors.Transparent,
                                    child: new Column(
                                        verticalDirection: VerticalDirection.up,
                                        children: new List<Widget>
                                        {
                                            new Padding(padding: EdgeInsets.only(bottom: 23)),
                                            new SizedBox(
                                                height: 35,
                                                child: new OutlineButton(
                                                    disabledBorderColor: CColors.White,
                                                    borderSide: new BorderSide(color: CColors.White, width: 5),
                                                    child:
                                                        new Text(data:"评论", style: new TextStyle(color: CColors.White, fontSize:14))
                                                    )
                                                ),
                                            new Padding(padding: EdgeInsets.only(bottom: 18)),
                                            new Listener(
                                                    onPointerDown: detail=>
                                                    {
                                                    },
                                                    child: new SizedBox(
                                                        width: 60,
                                                        height:60,
                                                        child: new RaisedButton(
                                                            shape: new CircleBorder(new BorderSide(color: CColors.White, width: 5)),
                                                            disabledColor: CColors.Transparent,
                                                            elevation: 0,
                                                            disabledElevation: 0,
                                                            color: CColors.Transparent,
                                                            child: !camera_pic ?
                                                                new Icon(
                                                                    color: CColors.FlatGreen,
                                                                    size: 24,
                                                                    icon: MyIcons.camera_button_mine
                                                                ) : null
                                                            )
                                                        )
                                                    ),
                                            new Padding(padding: EdgeInsets.only(top: 9)),
                                            new AnimatedPadding(
                                                padding: camera_pic ? EdgeInsets.fromLTRB(21.5f, 0, -21.5f, 0) : EdgeInsets.fromLTRB(-21.5f, 0, 21.5f, 0),
                                                duration: new System.TimeSpan(0,0,0,0,100),
                                                curve: Curves.easeInOut,
                                                child: new Container(
                                                    width: 111,
                                                    padding: EdgeInsets.only(left: 15, right: 15),
                                                    child: new Container(
                                                        height: 21,
                                                        width: 71,
                                                        child: new Row(
                                                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                                            children: new List<Widget>
                                                            {
                                                                new Listener(
                                                                    onPointerDown: detail => {if(!camera_pic) setState(()=>{camera_pic = true; }); },
                                                                    child: new Text(data: "照片", style: camera_pic ? CTextStyle.strength: CTextStyle.normal)),
                                                                new Listener(
                                                                    onPointerDown: detail => {if(camera_pic) setState(()=>{camera_pic = false; }); },
                                                                    child: new Text(data: "视频", style: !camera_pic ? CTextStyle.strength: CTextStyle.normal)),
                                                            })
                                                        )
                                                    )
                                                ),
                                        })
                                    )
                                )
                            ),
                        new Align(
                            alignment: Alignment.topLeft,
                            child: new IconButton(
                            color: CColors.Transparent,
                            iconSize: 24,
                            onPressed: () =>
                            {
                                BuildContext lastContext = Globals.instance.contextStack.Pop();
                                Navigator.pop(lastContext);
                            },
                            icon: new Icon(
                                icon: MyIcons.back_mine,
                                color: CColors.Black
                                )
                            )
                        ),
                        new Align(
                            alignment: new Alignment(0.95f, -0.98f),
                            child: new OutlineButton(
                                onPressed: () =>
                                {
                                    Globals.instance.contextStack.Push(context);
                                    //Globals.instance.arEffect.SetActive(true);
                                    Globals.instance.arBase.SetAREffectState(true);
                                    Globals.instance.arBase.EditMap();
                                    //Globals.instance.arEffect.GetComponent<AREffect.AREffectManager>().EditMap();

                                    Navigator.push(context, new MaterialPageRoute(builder: (_) =>
                                        {
                                            return new StoreProvider<AppState>(
                                                store: StoreProvider.store,
                                                new MaterialApp(
                                                    home: new SettingScreenConnector()
                                                )
                                            );
                                        }));
                                },
                                disabledBorderColor: CColors.White,
                                borderSide: new BorderSide(color: CColors.White, width: 5),
                                //color: CColors.Transparent,
                                child: new Text(data:"二次创作", style: new TextStyle(color: CColors.White, fontSize:16)))
                        ),
                    }
                )
            );
        }
    }
}
