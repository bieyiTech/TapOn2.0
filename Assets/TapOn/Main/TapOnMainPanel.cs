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
using UnityEngine.Networking;
using Unity.UIWidgets.async;
using TapOn.Utils;

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
            
            //if (Globals.instance == null) Debug.LogError("Global instance is null!");
            //if (Globals.instance.bmob == null) Debug.LogError("Global bmob is null!");
            if (Globals.instance.bmob == null)
            {
                GameObject t = GameObject.Find("Config");
                if (t == null) Debug.LogError("t is null");
                else
                {
                    Globals.instance.bmob = t.GetComponent<BmobUnity>();
                }
            }
            if (Globals.instance.bmob == null) Debug.LogError("Global bmob is null after!");
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
            FontManager.instance.addFont(Resources.Load<Font>("font/UnauthorizedIcon"), "UnauthorizedIcon");
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
                height: 73,
                
                child: new Column(children: new List<Widget>
                {
                    new Container(
                        decoration: new BoxDecoration(border: new Border(top: new BorderSide(color: CColors.Transparent, width: 0), bottom:new BorderSide(color: CColors.White, width: 0)), color: CColors.Transparent, image: new DecorationImage(fit: BoxFit.cover, image: new AssetImage("texture/bottomframe"))),
                        child:  new Row(
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
                                    new Padding(padding: EdgeInsets.only(top: 26)),
                                    new Icon(icon: MyIcons.map_mine, size: 24, color: _currentIndex == 0 ? CColors.IconGreen : CColors.IconBlack),
                                    new Padding(padding: EdgeInsets.only(top:2)),
                                    new Text(data: "地图", style: new TextStyle(fontSize: 10, color: CColors.Black)),
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
                                    new Padding(padding: EdgeInsets.only(top: 26)),
                                    new Icon(icon: MyIcons.list_mine, size: 24, color: _currentIndex == 1 ? CColors.IconGreen : CColors.IconBlack),
                                    new Padding(padding: EdgeInsets.only(top:2)),
                                    new Text(data: "榜单", style: new TextStyle(fontSize: 10, color: CColors.Black)),
                                })
                            )
                        ),
                        new Expanded(
                            flex: 5,
                            child: new Padding(padding: EdgeInsets.only(top: 9), child:
                            new SizedBox(
                                height: 54,
                                width: 54,
                                child: new FlatButton(
                                    onPressed: () =>
                                    {
                                        Globals.instance.contextStack.Push(context);
                                        //Globals.instance.map.SetActive(false);
                                        GameObject[] t = GameObject.FindGameObjectsWithTag("mark");
                                        foreach (GameObject mark in t)
                                            mark.SetActive(false);
                                        Globals.instance.arEffect.SetActive(true);
                                        Globals.instance.arEffect.GetComponent<AREffectManager>().CreateAndEditMap();
                                        Navigator.push(context, new MaterialPageRoute(builder: (_) =>
                                        {
                                            return new StoreProvider<AppState>(
                                                store: StoreProvider.store,
                                                new MaterialApp(
                                                    home: new SettingScreenConnector()
                                                )
                                            );
                                        }));

                                        
                                    },
                                    shape: new CircleBorder(),
                                    color: CColors.FlatGreen,
                                    disabledColor: CColors.FlatGreen,
                                    child: new Text(
                                        data: "埋下\n回忆",
                                        style: new TextStyle(fontSize: 12, color: CColors.White))
                                    )
                                )
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
                                    new Padding(padding: EdgeInsets.only(top: 26)),
                                    new Icon(icon: MyIcons.message_mine, size: 24, color: _currentIndex == 2 ? CColors.IconGreen : CColors.IconBlack),
                                    new Padding(padding: EdgeInsets.only(top:2)),
                                    new Text(data: "消息", style: new TextStyle(fontSize: 10, color: CColors.Black)),
                                })
                            )
                            ),
                        //new Padding(padding: EdgeInsets.only(right: 5)),
                        new Expanded(
                            flex: 5,
                            child: new Listener(
                                onPointerDown:detail =>
                                {
                                    //cn.bmob.response.UploadCallbackData ba = await Globals.instance.bmob.FileUploadTaskAsync(new cn.bmob.io.BmobLocalFile("E:/easyAR_sample/TapOn2.0/Assets/TapOn/Resources/texture/namecard.jpg"));
                                    /*Globals.instance.bmob.FileUpload("E:\\easyAR_sample\\TapOn2.0\\Assets\\TapOn\\Resources\\texture\\namecard.jpg", (resp,exception)=>
                                    {if(exception != null) {Debug.LogError(exception.Message);return; } Debug.Log(resp.filename); }
                                    );*/
                                    Window.instance.startCoroutine(test());
                                    setState(()=>{if(_currentIndex != 3) _currentIndex = 3; });
                                },
                                child: new Column(children: new List<Widget>
                                {
                                    new Padding(padding: EdgeInsets.only(top: 26)),
                                    new Icon(icon: MyIcons.mine_mine, size: 24, color: _currentIndex == 3 ? CColors.IconGreen : CColors.IconBlack),
                                    new Padding(padding: EdgeInsets.only(top:2)),
                                    new Text(data: "我的", style: new TextStyle(fontSize: 10, color: CColors.Black)),
                                })
                            )
                        ),
                    })
                        ),
                    new Container(constraints: new Unity.UIWidgets.rendering.BoxConstraints(maxHeight:73), color: CColors.White),
                })
                );
        }

        public IEnumerator test()
        {
            UnityWebRequest wr = new UnityWebRequest("https://api.bmob.cn/2/files/myTest.jpg", "POST");
            wr.SetRequestHeader("X-Bmob-Application-Id", "694024c993688a00b5707fba73ab8551");
            wr.SetRequestHeader("X-Bmob-REST-API-Key", "60ad19f4362c54aa3da42a41282cb369");
            wr.SetRequestHeader("Content-Type", "application/x-jpg");
            wr.uploadHandler = new UploadHandlerFile("E:\\easyAR_sample\\TapOn2.0\\Assets\\TapOn\\Resources\\texture\\namecard.jpg");
            wr.downloadHandler = new DownloadHandlerBuffer();
            yield return wr.SendWebRequest();
            if(wr.isHttpError || wr.isNetworkError)
            {
                Debug.LogError(wr.error + "\n" + wr.downloadHandler.text);
            }
            else
            {
                Debug.Log("bmob return " + wr.downloadHandler.text);
                Restful_FileUpLoadCallBack t = TapOnUtils.fileUpLoadCallBackfromJson(wr.downloadHandler.text);
                Debug.Log(t.filename);
                Debug.Log(t.url);
            }
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
                /*body: new PageView(
                    onPageChanged: value=> 
                    {
                        setState(() => { _currentIndex = value; });
                    },
                    controller: pc,
                    children: page
                    ),*/
                body: page[_currentIndex],
                bottomNavigationBar: _mineBottomBar()
            );
        }
    }
}
