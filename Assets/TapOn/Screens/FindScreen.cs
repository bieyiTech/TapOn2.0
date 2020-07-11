using System.Collections;
using System.Collections.Generic;
using TapOn.Constants;
using TapOn.Models.ActionModels;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
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

        private Widget imagePreview()
        {
            if(preview)
                return new Container(width: 300, height: 300, color: CColors.Blue);
            return new Container();
        }
        public override Widget build(BuildContext context)
        {
            return new Container(
                color: CColors.White, 
                child: new Unity.UIWidgets.widgets.Stack(
                    children: new List<Widget>
                    {
                        new Align(
                            alignment: Alignment.topLeft,
                            child: new IconButton(
                                onPressed: () =>
                                {
                                    Navigator.pop(Prefabs.homeContext);
                                },
                                icon: new Icon(
                                    size: 60,
                                    icon: MyIcons.tab_home_fill
                                )
                            )
                        ),
                        new Align(
                            alignment: Alignment.bottomCenter,
                            child: new Column(
                                verticalDirection: VerticalDirection.up,
                                children: new List<Widget>
                                {
                                    new GestureDetector(
                                        onTapDown: detail =>
                                        {
                                            setState(()=>{preview = true; });
                                        },
                                        onTapCancel: () =>
                                        {
                                            setState(()=>{preview = false; });
                                        },
                                        onTapUp: detail =>
                                        {
                                            setState(()=>{preview = false; });
                                        },
                                        child:new Container(
                                            decoration: new BoxDecoration(
                                                color: CColors.Red,
                                                borderRadius: BorderRadius.all(90)
                                                ),
                                            child: new Icon(
                                                icon: MyIcons.eye,
                                                size: 90
                                                )
                                            )
                                        ),
                                    imagePreview(),
                                }
                            )
                        )       
                    }
                ) 
            );
        }
    }
}
