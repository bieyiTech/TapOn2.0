using System.Collections;
using System.Collections.Generic;
using TapOn.Constants;
using TapOn.Models.ActionModels;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Screens
{
    public class SettingScreenConnector : StatelessWidget
    {
        public SettingScreenConnector(
            Key key = null
        ) : base(key: key)
        {
            Debug.LogError("343");
        }
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, SettingScreenViewModel>(
                converter: state =>
                {
                    return new SettingScreenViewModel
                    {

                    };
                },
                builder: (context1, viewModel, dispatcher) =>
                {
                    var actionModel = new SettingScreenActionModel
                    {

                    };
                    return new SettingScreen(viewModel: viewModel, actionModel: actionModel);
                }
            );
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

        public class _SettingScreenState : State<SettingScreen>
        {
            public override Widget build(BuildContext context)
            {
                Debug.Log("#");
                return new Column(
                    children: new List<Widget>()
                    {
                        new Container(child: new Text("4545")),
                        new BottomNavigationBar(
                            type: BottomNavigationBarType.fix,
                            items: new List<BottomNavigationBarItem>()
                            {
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.add)),
                                    new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.add)),
                                    new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.add)),
                            }
                            ),
                    }
                    );
            }
        }
    }
}
