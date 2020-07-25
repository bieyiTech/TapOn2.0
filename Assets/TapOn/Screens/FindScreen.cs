using System.Collections;
using System.Collections.Generic;
using TapOn.Constants;
using TapOn.Models.ActionModels;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.widgets;
using UnityEngine;

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
            if(preview)
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
                            alignment: Alignment.topLeft,
                            child: new BackButton()
                        ),
                        new Align(
                            alignment: new Alignment(0.95f, -0.98f),
                            child: new OutlineButton(
                                disabledBorderColor: CColors.White,
                                borderSide: new BorderSide(color: CColors.White, width: 5),
                                //color: CColors.Transparent,
                                child: new Text(data:"二次创作", style: new TextStyle(color: CColors.White, fontSize:16)))
                        ),
                        new Align(
                            alignment: Alignment.bottomCenter,
                            child: new GestureDetector(
                                onPanEnd: detail =>
                                {
                                    if (detail.velocity.pixelsPerSecond.dx < 0 && camera_pic)
                                        setState(()=>{camera_pic = false; });
                                    if (detail.velocity.pixelsPerSecond.dx > 0 && !camera_pic)
                                        setState(()=>{camera_pic = true; });
                                },
                                child: new Container(
                                    width: 100,
                                    height: 200,
                                    child: new Column(
                                        //verticalDirection: VerticalDirection.up,
                                        children: new List<Widget>
                                        {
                                            new AnimatedPadding(
                                                padding: camera_pic ? EdgeInsets.fromLTRB(25, 0, -25, 0) : EdgeInsets.fromLTRB(-25, 0, 25, 0),
                                                duration: new System.TimeSpan(0,0,0,0,300),
                                                curve: Curves.easeInOut,
                                                child: new Row(
                                                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                                                    children: new List<Widget>
                                                    {
                                                        new Text(data: "照片", style: camera_pic ? CTextStyle.strength: CTextStyle.normal),
                                                        new Text(data: "视频", style: !camera_pic ? CTextStyle.strength: CTextStyle.normal),
                                                    })
                                                ),
                                            new Padding(padding: EdgeInsets.only(top: 10)),
                                            new Align(
                                                child: new Listener(
                                                    onPointerDown: detail=>
                                                    {
                                                    },
                                                    child: new RaisedButton(
                                                        shape: new CircleBorder(new BorderSide(color: CColors.White, width: 8)),
                                                        elevation: 0,
                                                        color: CColors.Transparent,
                                                        child: new IconButton(
                                                                iconSize: 48,
                                                                icon: new Icon(
                                                                    color: CColors.White,
                                                                    size: 36,
                                                                    icon: MyIcons.upload
                                                                )
                                                            )
                                                        )
                                                    )
                                                ),
                                            new Padding(padding: EdgeInsets.only(top: 30)),
                                            new OutlineButton(
                                                disabledBorderColor: CColors.White,
                                                borderSide: new BorderSide(color: CColors.White, width: 5),
                                                //color: CColors.Transparent,
                                                child: new Text(data:"评论", style: new TextStyle(color: CColors.White, fontSize:14)))

                                        })
                                    )
                                )
                            ),       
                    }
                ) 
            );
        }
    }
}
