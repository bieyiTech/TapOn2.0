using System.Collections.Generic;
using RSG;
using TapOn.Api;
using TapOn.Constants;
using TapOn.Models.DataModels;
using TapOn.Models.States;
using TapOn.Redux;
using TapOn.Screens;
using TencentMap.API;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;


    public class testBar : UIWidgetsPanel
    {

        protected override Widget createWidget()
        {
            return new MaterialApp(
                showPerformanceOverlay: false,
                home: new MaterialNavigationBarWidget());
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        Screen.fullScreen = false;
        Screen.orientation = ScreenOrientation.Portrait;
        GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
        MapApi.map = mapObject.GetComponent<MapController>();
        MapApi.mapEnd = mapObject.GetComponent<MapEnd>();
        FontManager.instance.addFont(Resources.Load<Font>("font/Material Icons"), "Material Icons");
            FontManager.instance.addFont(Resources.Load<Font>("font/Outline Material Icons"), "Outline Material Icons");
            FontManager.instance.addFont(Resources.Load<Font>("font/Roboto-Regular"), "Roboto-Regular");
            FontManager.instance.addFont(Resources.Load<Font>("font/Roboto-Medium"), "Roboto-Medium");
            FontManager.instance.addFont(Resources.Load<Font>("font/Roboto-Bold"), "Roboto-Bold");
            FontManager.instance.addFont(Resources.Load<Font>("font/PingFangSC-Regular"), "PingFangSC-Regular");
            FontManager.instance.addFont(Resources.Load<Font>("font/PingFangSC-Medium"), "PingFangSC-Medium");
            FontManager.instance.addFont(Resources.Load<Font>("font/PingFangSC-Semibold"), "PingFangSC-Semibold");
            FontManager.instance.addFont(Resources.Load<Font>("font/Menlo-Regular"), "Menlo");
            FontManager.instance.addFont(Resources.Load<Font>("font/iconFont"), "iconfont");
        }

    static PageRouteFactory pageRouteBuilder
    {
        get
        {
            return (settings, builder) =>
                new PageRouteBuilder(
                    settings: settings,
                    (context, animation, secondaryAnimation) => builder(context)
                );
        }
    }
}

    class MaterialNavigationBarWidget : StatefulWidget
    {
        public MaterialNavigationBarWidget(Key key = null) : base(key)
        {
        }

        public override State createState()
        {
            return new MaterialNavigationBarWidgetState();
        }
    }

    class MaterialNavigationBarWidgetState : SingleTickerProviderStateMixin<MaterialNavigationBarWidget>
    {
        int _currentIndex = 0;
    Widget page = new StoreProvider<AppState>(
                store: StoreProvider.store,
                new MaterialApp(
                    home: new MapScreenConnector()
                //pageRouteBuilder: pageRouteBuilder
                )
            );

    public MaterialNavigationBarWidgetState()
        {
        }

        public override Widget build(BuildContext context)
        {
            return new Scaffold(
                backgroundColor: CColors.Transparent,
                //floatingActionButtonLocation: FloatingActionButtonLocation.centerDocked,
                //floatingActionButton: new FloatingActionButton(child: new Icon(icon: Mark.mark_icon)),
                appBar: new AppBar(title: new Text("TapOn")),
                body:page,
                bottomNavigationBar: new BottomAppBar(
                    //shape: new CircularNotchedRectangle(),
                    color: CColors.White,
                    child: new BottomNavigationBar(
                        type: BottomNavigationBarType.shifting,
                            // type: BottomNavigationBarType.fix,
                            items: new List<BottomNavigationBarItem> {
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.tab_home_fill, size: 30),
                                    title: new Text("地图"),
                                    activeIcon: new Icon(icon: MyIcons.tab_home_fill, size: 50)
                                ),
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.camera_alt, size: 30),
                                    title: new Text("印记"),
                                    activeIcon: new Icon(icon: MyIcons.camera_alt, size: 50)
                                    //backgroundColor: Colors.blue
                                ),
                                new BottomNavigationBarItem(
                                    icon: new Icon(MyIcons.tab_messenger_fill, size: 30),
                                    title: new Text("消息"),
                                    activeIcon: new Icon(MyIcons.tab_messenger_fill, size: 50)
                                    //backgroundColor: Colors.blue
                                ),
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.tab_mine_fill, size: 30),
                                    title: new Text("我的"),
                                    activeIcon: new Icon(icon: MyIcons.tab_mine_fill, size: 50)
                                    //backgroundColor: Colors.blue
                                ),
                            },
                            selectedItemColor: CColors.Black,
                            unselectedItemColor: CColors.Black,
                            currentIndex: this._currentIndex,
                            onTap: (value) => { this.setState(() => { this._currentIndex = value; }); }
                    )
                )
            );
        }
    }
