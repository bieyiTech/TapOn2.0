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
            return new Column(children: new List<Widget>
        {
            new Text(data: "个人中心", textAlign: TextAlign.center),
            new CircleAvatar(radius: 60),
            new Text(data: "dolly", textAlign: TextAlign.center),
            new Text(data: "Hear me roar", textAlign: TextAlign.center),
        });
        }

        private Widget _middle()
        {
            return new Container(
                decoration: new BoxDecoration(borderRadius: BorderRadius.all(30)),
                child: new Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: new List<Widget>
                    {
                        new Icon(icon: MyIcons.camera_alt, color: CColors.Black),
                        new Text(data: "留些足迹"),
                    }));
        }

        private Widget _bottom()
        {
            return new Column(children: new List<Widget>
            {
                new Row(children: new List<Widget>{new Text(data: "我的印记"), }),
                new Divider(color: CColors.Grey),
                new Row(children: new List<Widget>{new Text(data: "我的钱包"), }),
                new Divider(color: CColors.Grey),
                new Row(children: new List<Widget>{new Text(data: "个人信息"), }),
                new Divider(color: CColors.Grey),
                new Row(children: new List<Widget>{new Text(data: "设置"), }),
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
                color: CColors.White,
                child: new Column(
                    children: new List<Widget>
                    {
                        _top(),
                        _middle(),
                        _bottom(),
                    })
                );
        }
    }
}
