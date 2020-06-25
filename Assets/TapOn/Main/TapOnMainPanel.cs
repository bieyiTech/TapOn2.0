using System.Collections;
using System.Collections.Generic;
using TapOn.Api;
using TapOn.Constants;
using TapOn.Models.States;
using TapOn.Redux;
using TapOn.Screens;
using TencentMap.API;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Main
{
    public class TapOnMainPanel : UIWidgetsPanel
    {
        public MapController map;
        protected override void OnEnable()
        {
            base.OnEnable();
            //Debuger.EnableLog = Config.enableDebug;
            Screen.fullScreen = false;
            Screen.orientation = ScreenOrientation.Portrait;
            //Window.onFrameRateCoolDown = CustomFrameRateCoolDown;
            LoadFonts();
            GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
            MapApi.map = mapObject.GetComponent<MapController>();
            MapApi.mapEnd = mapObject.GetComponent<MapEnd>();
            if (MapApi.map != null)
                Debug.Log("get map !");
            Debug.Log("width:" + Screen.width + " height:" + Screen.height);
        }

        static void LoadFonts()
        {
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

        protected override Widget createWidget()
        {
            return new MaterialApp(
                showPerformanceOverlay: false,
                home: new MainScaffold());
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

    class MainScaffold : StatefulWidget
    {
        public MainScaffold(Key key = null) : base(key)
        {
        }

        public override State createState()
        {
            return new MainScaffoldState();
        }
    }

    class MainScaffoldState : SingleTickerProviderStateMixin<MainScaffold>
    {
        int _currentIndex = 0;
        List<Widget> page = new List<Widget>()
        {
            new StoreProvider<AppState>(
                store: StoreProvider.store,
                new MaterialApp(
                    home: new MapScreenConnector()
                )
            ),
            new StoreProvider<AppState>(
                store: StoreProvider.store,
                new MaterialApp(
                    home: new SettingScreenConnector()
                )
            ),
        };

        public Widget _bottomNavigationBar()
        {
            return new BottomAppBar(
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
                            ),
                            new BottomNavigationBarItem(
                                icon: new Icon(MyIcons.tab_messenger_fill, size: 30),
                                title: new Text("消息"),
                                activeIcon: new Icon(MyIcons.tab_messenger_fill, size: 50)
                            ),
                            new BottomNavigationBarItem(
                                icon: new Icon(icon: MyIcons.tab_mine_fill, size: 30),
                                title: new Text("我的"),
                                activeIcon: new Icon(icon: MyIcons.tab_mine_fill, size: 50)
                            ),
                        },
                        selectedItemColor: CColors.Black,
                        unselectedItemColor: CColors.Black,
                        currentIndex: this._currentIndex,
                        onTap: (value) => { Debug.LogError(value); this.setState(() => { this._currentIndex = value; }); }
                )
            );
        }

        public AppBar _appBar()
        {
            return new AppBar(
                backgroundColor: CColors.White,
                centerTitle: true,
                title: new Text(
                    data: "TapOn",
                    style: CTextStyle.H2
                )
            );
        }

        public override Widget build(BuildContext context)
        {
            return new Scaffold(
                backgroundColor: CColors.Transparent,
                //floatingActionButtonLocation: FloatingActionButtonLocation.centerDocked,
                //floatingActionButton: new FloatingActionButton(child: new Icon(icon: Mark.mark_icon)),
                appBar: _appBar(),
                body: _currentIndex > 1 ? page[0] : page[_currentIndex],
                bottomNavigationBar: _bottomNavigationBar()
            );
        }
    }
}
