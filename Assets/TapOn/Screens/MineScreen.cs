using System.Collections;
using System.Collections.Generic;
using TapOn.Constants;
using UIWidgetsGallery.gallery;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Screens
{
    public class MineScreen : StatelessWidget
    {
        List<Widget> _bottomItems = new List<Widget>
        {
            new Row(children: new List<Widget>{new Text(data: "我的印记"), }),
            new Row(children: new List<Widget>{new Text(data: "我的钱包"), }),
            new Row(children: new List<Widget>{new Text(data: "个人信息"), }),
            new Row(children: new List<Widget>{new Text(data: "设置"), }),
        };
        private Widget _top()
        {
            return new Container(
                width: 1000,
                color: CColors.White,
                child: new Column(children: new List<Widget>
                {
                    new Padding(padding: EdgeInsets.only(top: 20),child:
                        new Text(
                            data: "个人中心",
                            textAlign: TextAlign.center,
                            style: new TextStyle(
                                color: CColors.Black,
                                fontSize: 20,
                                fontFamily: "Roboto-Bold",
                                fontWeight: FontWeight.w100,
                                decoration: TextDecoration.none
                                )
                            )
                        ),
                    new Padding(
                        padding: EdgeInsets.only(top: 20),
                        child: new CircleAvatar(radius: 50, backgroundColor: CColors.Black)),
                    new Text(
                        data: "dolly", 
                        textAlign: TextAlign.center,
                        style: new TextStyle(
                            color: CColors.Black,
                            fontSize: 28,
                            fontFamily: "Roboto-Bold",
                            fontWeight: FontWeight.w600,
                                decoration: TextDecoration.none
                            )
                        ),
                    //new Container(height: 20, width: 300, color: CColors.Black),
                    new Text(
                        data: "Hear me roar",
                        textAlign: TextAlign.center,
                        style: new TextStyle(
                            color: CColors.Black,
                            fontSize: 16,
                            fontFamily: "Roboto-Bold",
                            fontWeight: FontWeight.w100,
                            decoration: TextDecoration.none
                            )
                        ),
                })
            );
        }

        private Widget _middle()
        {
            return new Container(
                margin: EdgeInsets.only(top: 10),
                padding: EdgeInsets.only(top: 6, bottom: 6),
                decoration: new BoxDecoration(
                    color: CColors.White,
                    border: Border.all(color: CColors.Black, width: 1),
                    borderRadius: BorderRadius.all(5)
                    ),
                child: new Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: new List<Widget>
                    {
                        new Icon(icon: MyIcons.camera_alt, color: CColors.Black),
                        new Padding(padding: EdgeInsets.only(left: 10)),
                        new Text(
                            data: "留些足迹",
                            style: new TextStyle(
                                color: CColors.Black,
                                fontSize: 20,
                                fontFamily: "Roboto-Bold",
                                fontWeight: FontWeight.w300,
                                decoration: TextDecoration.none
                                )
                            ),
                    }));
        }

        private Widget _bottom()
        {
            return new Column(children: new List<Widget>
            {
                new Container(
                    color: CColors.White,
                    padding: EdgeInsets.all(10),
                    child: new Row(
                    children: new List<Widget>
                    {
                        new Icon(icon: MyIcons.picture, color:CColors.Black),
                        new Padding(padding: EdgeInsets.only(left: 10)),
                        new Text(
                            data: "我的印记",
                            style: new TextStyle(
                                color: CColors.Black,
                                fontSize: 16,
                                fontFamily: "Roboto-Bold",
                                fontWeight: FontWeight.w200,
                                decoration: TextDecoration.none
                                )
                            ),
                    })
                ),
                new Divider(color: CColors.Grey , height: 1),
                new Container(
                    color: CColors.White,
                    padding: EdgeInsets.all(10),
                    child: new Row(
                    children: new List<Widget>
                    {
                        new Icon(icon: MyIcons.eye, color:CColors.Black),
                        new Padding(padding: EdgeInsets.only(left: 10)),
                        new Text(
                            data: "我的钱包",
                            style: new TextStyle(
                                color: CColors.Black,
                                fontSize: 16,
                                fontFamily: "Roboto-Bold",
                                fontWeight: FontWeight.w200,
                                decoration: TextDecoration.none
                                )
                            ),
                    })
                ),
                new Divider(color: CColors.Grey, height: 1),
                new Container(
                    color: CColors.White,
                    padding: EdgeInsets.all(10),
                    child: new Row(
                    children: new List<Widget>
                    {
                        new Icon(icon: MyIcons.tab_mine_fill, color:CColors.Black),
                        new Padding(padding: EdgeInsets.only(left: 10)),
                        new Text(
                            data: "认证中心",
                            style: new TextStyle(
                                color: CColors.Black,
                                fontSize: 16,
                                fontFamily: "Roboto-Bold",
                                fontWeight: FontWeight.w200,
                                decoration: TextDecoration.none
                                )
                            ),
                    })
                ),
                new Divider(color: CColors.Grey, height: 1),
                new Container(
                    color: CColors.White,
                    padding: EdgeInsets.all(10),
                    child: new Row(
                    children: new List<Widget>
                    {
                        new Icon(icon: MyIcons.settings, color:CColors.Black),
                        new Padding(padding: EdgeInsets.only(left: 10)),
                        new Text(
                            data: "设置",
                            style: new TextStyle(
                                color: CColors.Black,
                                fontSize: 16,
                                fontFamily: "Roboto-Bold",
                                fontWeight: FontWeight.w200,
                                decoration: TextDecoration.none
                                )
                            ),
                    })
                ),
            });
            /*return ListView.seperated(
                    itemCount: 4,
                    itemBuilder: (context, index) =>
                    {
                        return _bottomItems[index];
                    },
                    separatorBuilder: (context, index) =>
                    {
                        return new Divider(color: CColors.Grey);
                    }
                    );*/
        }
        public override Widget build(BuildContext context)
        {
            return new Container(
                color: CColors.Grey80,
                child: new Column(
                    children: new List<Widget>
                    {
                        _top(),
                        _middle(),
                        new Padding(padding: EdgeInsets.only(top: 10)),
                        _bottom(),
                    })
                );
        }
    }
}
