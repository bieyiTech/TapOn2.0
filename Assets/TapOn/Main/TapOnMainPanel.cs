using cn.bmob.api;
using cn.bmob.tools;
using System.Collections;
using System.Collections.Generic;
using TapOn.Api;
using TapOn.Constants;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using TapOn.Redux;
using TapOn.Screens;
using TencentMap.API;
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using AREffect;
using Image = Unity.UIWidgets.widgets.Image;

namespace TapOn.Main
{
    public class TapOnMainPanel : UIWidgetsPanel
    {
        public MapController map;
        public Globals prefabs;
        protected override void OnEnable()
        {
            base.OnEnable();
            //Debuger.EnableLog = Config.enableDebug;
            Screen.fullScreen = false;
            Screen.orientation = ScreenOrientation.Portrait;
            //Window.onFrameRateCoolDown = CustomFrameRateCoolDown;
            LoadFonts();
            BmobDebug.Register(print);
            BmobDebug.level = BmobDebug.Level.TRACE;
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
            FontManager.instance.addFont(Resources.Load<Font>("font/myicon"), "myicon");
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
        PageController pc= new PageController(initialPage: 0);

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
                    home: new MineScreen()
                )
            ),
            new StoreProvider<AppState>(
                store: StoreProvider.store,
                new MaterialApp(
                    home: new MineScreen()
                )
            ),
            new StoreProvider<AppState>(
                store: StoreProvider.store,
                new MaterialApp(
                    home: new MineScreen()
                )
            ),
        };

        public Widget _mineBottomBar()
        {
            return new Container(
                height: 50,
                decoration: new BoxDecoration(image: new DecorationImage(image: new AssetImage("texture/bottomframe"))),
                child: new Row(
                    mainAxisAlignment: Unity.UIWidgets.rendering.MainAxisAlignment.spaceAround,
                    children: new List<Widget>
                    {
                        new Expanded( 
                            flex: 5,
                            child:  new Listener(
                                onPointerDown: detail => 
                                {
                                    setState(()=>{if(_currentIndex != 0) _currentIndex = 0; });
                                }, 
                                child: new Column(children: new List<Widget>
                                {
                                    new Padding(padding: EdgeInsets.only(top: 15)),
                                    new Icon(icon: MyIcons.word_mine, size: 16, color: _currentIndex == 0 ? CColors.SecondaryPink : CColors.Black),
                                    new Padding(padding: EdgeInsets.only(top:5)),
                                    new Text(data: "地图", style: new TextStyle(fontSize: 8)),
                                })
                            )
                        ),
                        //new Padding(padding: EdgeInsets.only(left: 15)),
                        new Expanded( 
                            flex: 4,
                            child: new Listener(
                                onPointerDown: detail =>
                                {
                                    setState(()=>{if(_currentIndex != 1) _currentIndex = 1; });
                                },
                                child: new Column(children: new List<Widget>
                                {
                                    new Padding(padding: EdgeInsets.only(top: 15)),
                                    new Icon(icon: MyIcons.word_mine, size: 16, color: _currentIndex == 1 ? CColors.SecondaryPink : CColors.Black),
                                    new Padding(padding: EdgeInsets.only(top:5)),
                                    new Text(data: "树洞", style: new TextStyle(fontSize: 8)),
                                })
                            )
                        ),
                        new Expanded( 
                            flex: 5,
                            child: new FlatButton(
                                onPressed: () =>
                                {
                                    Globals.instance.contextStack.Push(context);
                                    //Globals.instance.map.SetActive(false);
                                    GameObject[] t = GameObject.FindGameObjectsWithTag("mark");
                                    foreach (GameObject mark in t)
                                        mark.SetActive(false);
                                    Navigator.push(context, new MaterialPageRoute(builder: (_) =>
                                    {
                                        return new StoreProvider<AppState>(
                                            store: StoreProvider.store,
                                            new MaterialApp(
                                                home: new SettingScreenConnector()
                                            )
                                        );
                                    }));

                                    Globals.instance.arEffect.SetActive(true);
                                    Globals.instance.arEffect.GetComponent<AREffectManager>().CreateAndEditMap();
                                },
                                shape: new CircleBorder(),
                                color: CColors.WeChatGreen,
                                disabledColor: CColors.WeChatGreen,
                                child: new Text(
                                    data: "埋下\n回忆", 
                                    style: new TextStyle(fontSize: 10, color: CColors.White))
                                )
                            ),
                        //new Padding(padding: EdgeInsets.only(left: -15)),
                        new Expanded( 
                            flex: 4,
                            child: new Listener(
                                onPointerDown: detail =>
                                {
                                    setState(()=>{if(_currentIndex != 2) _currentIndex = 2; });
                                },
                                child: new Column(children: new List<Widget>
                                {
                                    new Padding(padding: EdgeInsets.only(top: 15)),
                                    new Icon(icon: MyIcons.word_mine, size: 16, color: _currentIndex == 2 ? CColors.SecondaryPink : CColors.Black),
                                    new Padding(padding: EdgeInsets.only(top:5)),
                                    new Text(data: "消息", style: new TextStyle(fontSize: 8)),
                                })
                            )
                            ),
                        //new Padding(padding: EdgeInsets.only(right: 5)),
                        new Expanded(
                            flex: 5,
                            child: new Listener(
                                onPointerDown: detail =>
                                {
                                    setState(()=>{if(_currentIndex != 3) _currentIndex = 3; });
                                },
                                child: new Column(children: new List<Widget>
                                {
                                    new Padding(padding: EdgeInsets.only(top: 15)),
                                    new Icon(icon: MyIcons.word_mine, size: 16, color: _currentIndex == 3 ? CColors.SecondaryPink : CColors.Black),
                                    new Padding(padding: EdgeInsets.only(top:5)),
                                    new Text(data: "我的", style: new TextStyle(fontSize: 8)),
                                })
                            )
                        ),
                    })
                );
        }

        public Widget _bottomNavigationBar()
        {
            return new BottomAppBar(
                shape: new CircularNotchedRectangle(),
                color: CColors.White,
                child: new BottomNavigationBar(
                    type: BottomNavigationBarType.fix,
                        // type: BottomNavigationBarType.fix,
                        items: new List<BottomNavigationBarItem> {
                            new BottomNavigationBarItem(
                                icon: new Icon(icon: MyIcons.tab_home_fill, size: 30),
                                title: new Text("地图")
                                //activeIcon: new Icon(icon: MyIcons.tab_home_fill, size: 50)
                            ),
                            /*new BottomNavigationBarItem(
                                icon: new Icon(icon: MyIcons.camera_alt, size: 30),
                                title: new Text("印记")
                                //activeIcon: new Icon(icon: MyIcons.camera_alt, size: 50)
                            ),*/
                            /*new BottomNavigationBarItem(
                                icon: new Icon(MyIcons.tab_messenger_fill, size: 30),
                                title: new Text("消息")
                                //activeIcon: new Icon(MyIcons.tab_messenger_fill, size: 50)
                            ),*/
                            new BottomNavigationBarItem(
                                icon: new Icon(icon: MyIcons.tab_mine_fill, size: 30),
                                title: new Text("我的")
                                //activeIcon: new Icon(icon: MyIcons.tab_mine_fill, size: 50)
                            ),
                        },
                        selectedItemColor: CColors.Green,
                        unselectedItemColor: CColors.Black,
                        currentIndex: this._currentIndex,
                        onTap: (value) => { if(value != _currentIndex) this.setState(() => { this._currentIndex = value; }); }
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

        /*public override Widget build(BuildContext context)
        {
            return new Container();
        }*/

        public override Widget build(BuildContext context)
        {
            Globals.instance.homeContext = context;
            return new Scaffold(
                backgroundColor: CColors.Transparent,
                /*floatingActionButtonLocation: FloatingActionButtonLocation.centerDocked,
                floatingActionButton: new FloatingActionButton(
                    elevation: 0,
                    onPressed: ()=>
                    {
                        Prefabs.instance.map.SetActive(false);
                        GameObject[] t = GameObject.FindGameObjectsWithTag("mark");
                        foreach (GameObject mark in t)
                            mark.SetActive(false);
                        Navigator.push(context, new MaterialPageRoute(builder: (_) =>
                        {
                            return new StoreProvider<AppState>(
                                store: StoreProvider.store,
                                new MaterialApp(
                                    home: new SettingScreenConnector()
                                )
                            );
                        }));
                        //运行create部分代码和edit代码
                        Prefabs.instance.arEffect.SetActive(true);
                        Prefabs.instance.arEffect.GetComponent<AREffectManager>().CreateAndEditMap();
                    },
                    backgroundColor: CColors.Red,
                    child: new Icon(
                        icon: MyIcons.camera_alt
                        )
                    ),*/
                //appBar: _appBar(),
                body: new PageView(
                    onPageChanged: value=> 
                    {
                        setState(() => { _currentIndex = value; });
                    },
                    controller: pc,
                    children: page
                    ),
                //body: _currentIndex > 1 ? page[0] : page[_currentIndex],
                bottomNavigationBar: _mineBottomBar()
            );
        }
    }
}
