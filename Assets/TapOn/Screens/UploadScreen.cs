using com.unity.uiwidgets.Runtime.rendering;
using System.Collections;
using System.Collections.Generic;
using TapOn.Constants;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.service;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.cupertino;
using UnityEngine;
using Stack = Unity.UIWidgets.widgets.Stack;
using Unity.UIWidgets.Redux;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using TapOn.Models.ActionModels;
using Unity.UIWidgets.foundation;

namespace TapOn.Screens
{
    public class UploadScreenConnector : StatelessWidget
    {
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, UploadScreenViewModel>(
                converter: state =>
                {
                    return new UploadScreenViewModel
                    {
                    };
                },
                builder: (context1, viewModel, dispatcher) =>
                {
                    var actionModel = new UploadScreenActionModel
                    {
                    };
                    return new UploadScreen(viewModel: viewModel, actionModel: actionModel);
                }
            );
        }
    }

    public class UploadScreen : StatefulWidget
    {
        public UploadScreen(
        UploadScreenViewModel viewModel = null,
        UploadScreenActionModel actionModel = null,
        Key key = null
            ) : base(key: key)
        {
            this.viewModel = viewModel;
            this.actionModel = actionModel;
        }

        public readonly UploadScreenViewModel viewModel;
        public readonly UploadScreenActionModel actionModel;

        public override State createState()
        {
            _UploadScreenState s = new _UploadScreenState();
            return s;
        }
    }
    public class _UploadScreenState : State<UploadScreen>
    {
        TextEditingController tec = new TextEditingController();
        List<byte[]> pictures = new List<byte[]>();
        byte[] mapShot;

        bool switchForAllPeople = true;
        bool switchForAllTime = true;
        private Widget _textField()
        {
            return new TextField(
                controller: tec,
                keyboardType: TextInputType.multiline,
                maxLines: 2,
                //autofocus: true,
                decoration: new InputDecoration(
                    contentPadding: EdgeInsets.symmetric(vertical: 20),
                    border: InputBorder.none,
                    hintText: "说点什么..."));
        }

        private Widget _listPicture()
        {
            return new Container(
                height: 100,
                child: GridView.builder(
                    gridDelegate: new SliverGridDelegateWithFixedCrossAxisCount(
                        crossAxisCount: 3,
                        childAspectRatio: 1.0f
                        ),
                    itemCount: pictures.Count == 3 ? 3 : pictures.Count + 1,
                    itemBuilder: (context, index) =>
                    {
                        if (index < pictures.Count)
                            return new Unity.UIWidgets.widgets.Image(image: new MemoryImage(bytes: pictures[index]));
                        else
                            return new Container(color: CColors.Grey80, child: new Icon(icon: MyIcons.add, size: 28, color: CColors.Black));
                    })
                );
        }

        private Widget _mapSnapshot()
        {
            return  new Unity.UIWidgets.widgets.Image(fit: BoxFit.cover, width: 1000, image: new NetworkImage(url: "https://bmob-cdn-28754.bmobpay.com/2020/07/14/79491070409e796180a00130e025e6c1.jpg"));
        }
        public override Widget build(BuildContext context)
        {
            return new Container(
                color: CColors.White,
                child: new Stack(
                    children: new List<Widget>
                    {
                    new Align(alignment: Alignment.topLeft, child: new BackButton()),
                    new Align(
                        alignment: new Alignment(0.95f, -0.98f),
                        child: new FlatButton(
                            //minSize: 24,
                            onPressed: ()=>{},
                            disabledColor: CColors.WeChatGreen,
                            color: CColors.WeChatGreen, 
                            child: new Text(data:"发布", style: new TextStyle(color: CColors.White, fontSize:18)))),
                    new Container(
                        margin: EdgeInsets.only(left: 20, right: 20),
                        child: new Column(
                            children: new List<Widget>
                            {
                                new Padding(padding: EdgeInsets.only(top: 40)),
                                _textField(),
                                new Padding(padding: EdgeInsets.only(top: 10)),
                                _listPicture(),
                                new Padding(padding: EdgeInsets.only(top: 10)),
                                _mapSnapshot(),
                                new Padding(padding: EdgeInsets.only(top: 10)),
                                new Row(
                                    mainAxisAlignment: Unity.UIWidgets.rendering.MainAxisAlignment.spaceBetween,
                                    children: new List<Widget>
                                    {
                                        new Text(
                                            data: "对所有人可见",
                                            style: new TextStyle(
                                                color: CColors.Black,
                                                fontSize: 18,
                                                fontFamily: "Roboto-regular",
                                                fontWeight: FontWeight.w100,
                                                decoration: TextDecoration.none
                                                )
                                            ),
                                        new CupertinoSwitch(
                                            value: switchForAllPeople,
                                            onChanged: value=>{ }),
                                    }),
                                new Padding(padding: EdgeInsets.only(top: 10)),
                                new Row(
                                    mainAxisAlignment: Unity.UIWidgets.rendering.MainAxisAlignment.spaceBetween,
                                    children: new List<Widget>
                                    {
                                        new Text(
                                            data: "所有时间可见",
                                            style: new TextStyle(
                                                color: CColors.Black,
                                                fontSize: 18,
                                                fontFamily: "Roboto-regular",
                                                fontWeight: FontWeight.w100,
                                                decoration: TextDecoration.none
                                                )
                                            ),
                                        new CupertinoSwitch(
                                            value: switchForAllTime,
                                            onChanged: value=>{ }),
                                    }),
                            })
                        ),
                    })
                );
        }
    }
}
