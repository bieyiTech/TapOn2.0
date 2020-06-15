using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.foundation;
using UnityEngine;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.material;
using Models.State;
using Models.ViewModel;
using Models.ActionModel;
using Redux.Actions;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Text = Unity.UIWidgets.widgets.Text;
using Color = Unity.UIWidgets.ui.Color;
using Components;
using Constants;

namespace Screens
{
    public class newScreenConnector : StatelessWidget
    {
        public newScreenConnector(
            Key key = null
        ) : base(key: key)
        {
            
        }
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, NewViewModel>(
                converter: state => new NewViewModel
                    {
                        name = state.newState.name,
                    },
                builder: (context1, viewModel, dispatcher) => {
                    var actionModel = new NewActionModel
                    {
                        changeName = text =>
                            dispatcher.dispatch(new ChangeNameAction { changeName = text }),
                    };
                    return new newScreen(viewModel: viewModel, actionModel: actionModel);
                }
            );
        }
    }

    public class newScreen :StatefulWidget
    {
        public newScreen(
            NewViewModel viewModel = null,
            NewActionModel actionModel = null,
            Key key = null
        ) : base(key: key)
        {
            this.viewModel = viewModel;
            this.actionModel = actionModel;
        }

        public readonly NewViewModel viewModel;
        public readonly NewActionModel actionModel;

        public override State createState()
        {
            return new _newScreenState();
        }
    }

    public class _newScreenState : State<newScreen>
    {
        public override Widget build(BuildContext context)
        {
            return new Container(
                height: 200,
                padding: EdgeInsets.all(10),
                decoration: new BoxDecoration(
                        color: new Color(0xFFEF1F7F),
                        border: Border.all(color: Color.fromARGB(255, 0xDF, 0x10, 0x70), width: 5)
                    ),
                child: new Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: new List<Widget>
                    {
                        new InputField(
                            decoration: new BoxDecoration(
                                color: new Color(0xFFFFFFFF),
                                border: Border.all(color: Color.fromARGB(255, 0xDF, 0x10, 0x70), width: 5),
                                borderRadius: BorderRadius.all(20)
                            ),
                            hintText:"changeName",
                            hintStyle: CTextStyle.PLargeBody5,
                            style: CTextStyle.PLargeBody,
                            autofocus: false,
                            height: 24,
                            cursorColor: CColors.PrimaryBlue,
                            maxLines: 1,
                            onChanged: text => this.widget.actionModel.changeName(text)
                        ),
                        new Text(
                            data: this.widget.viewModel.name
                        )
                    }
                )
            );
        }
    }
}
