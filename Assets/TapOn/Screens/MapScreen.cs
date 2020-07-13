using cn.bmob.response;
using RSG;
using System.Collections;
using System.Collections.Generic;
using TapOn.Api;
using TapOn.Constants;
using TapOn.Models;
using TapOn.Models.ActionModels;
using TapOn.Models.DataModels;
using TapOn.Models.States;
using TapOn.Models.ViewModels;
using TapOn.Redux.Actions;
using TencentMap.CoordinateSystem;
using UIWidgetsGallery.gallery;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Stack = Unity.UIWidgets.widgets.Stack;

namespace TapOn.Screens
{
    public class MapScreenConnector : StatelessWidget
    {
        public MapScreenConnector(
            Key key = null
        ) : base(key: key)
        {

        }
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, MapScreenViewModel>(
                converter: state =>
                {
                    //Debug.LogError("update time: " + Time.realtimeSinceStartup);

                    return new MapScreenViewModel
                    {
                        marks = state.mapState.marks,
                        positions = state.mapState.positions,
                        pixelPositions = state.mapState.pixelPositions,
                    };
                },
                builder: (context1, viewModel, dispatcher) =>
                {
                    var actionModel = new MapScreenActionModel
                    {
                        mapMoveOffsetX = x =>
                            dispatcher.dispatch(new MapHorizontalDragAction { offset = x }),
                        mapMoveOffsetY = y =>
                            dispatcher.dispatch(new MapVerticalDragAction { offset = y }),
                        mapZoom = s =>
                            dispatcher.dispatch(new MapZoomAction { scale = s }),
                        markPositionUpdate = u =>
                            dispatcher.dispatch(new UpdatePositionsAction { update = u }),
                        selectMark = u =>
                            dispatcher.dispatch(new SelectMarkAction { pos = u }),
                        addMarkJustLoading = l =>
                            dispatcher.dispatch(new AddMarkJustLoadingAction { newMarks = l }),
                        moveMap = () => dispatcher.dispatch<IPromise>(Actions.moveMap()),
                        zoomMap = () => dispatcher.dispatch<IPromise>(Actions.zoomMap()),
                        changeMark = () => dispatcher.dispatch<IPromise>(Actions.changeMark()),
                        loadMark = () => dispatcher.dispatch<IPromise>(Actions.loadMark()),
                    };
                    return new MapScreen(viewModel: viewModel, actionModel: actionModel);
                }
            );
        }
    }

    public class MapScreen : StatefulWidget
    {
        public MapScreen(
            MapScreenViewModel viewModel = null,
            MapScreenActionModel actionModel = null,
            Key key = null
        ) : base(key: key)
        {
            this.viewModel = viewModel;
            this.actionModel = actionModel;
        }

        public readonly MapScreenViewModel viewModel;
        public readonly MapScreenActionModel actionModel;

        public override State createState()
        {
            _MapScreenState s = new _MapScreenState();
            return s;
        }
    }

    public class _MapScreenState : SingleTickerProviderStateMixin<MapScreen>
    {
        private List<Vector2> allPos = new List<Vector2>();
        private int markSize = 40;
        public int _currentIndex = 0;
        public List<Widget> _buildMark()
        {
            List<Widget> allMark = new List<Widget>();
            //if(this.widget.viewModel.positions.Count>0)
            //Debug.LogError("vertex : " + this.widget.viewModel.positions[0].x + ", " + this.widget.viewModel.positions[0].y);


            for (int i=0;i<this.widget.viewModel.positions.Count;i++)
            {                /*FractionalOffset tt = FractionalOffset.fromOffsetAndSize
                    (new Unity.UIWidgets.ui.Offset(this.widget.viewModel.positions[i].x, this.widget.viewModel.positions[i].y),
                    new Unity.UIWidgets.ui.Size(40, 40));*/
                //Debug.LogError("x: " + tt.dx + " y: " + tt.dy);
                allMark.Add(new Align(
                    alignment: new FractionalOffset(this.widget.viewModel.positions[i].x, this.widget.viewModel.positions[i].y),
                    //alignment: new FractionalOffset(0,0),
                    child: new Icon(
                        icon: MyIcons.mark_icon,
                        size: markSize,
                        color: CColors.Red
                    )
                    ));
               /*allMark.Add(new Positioned(
                    child: new Icon(
                        icon: MyIcons.mark_icon,
                        size: markSize,
                        color: CColors.Red
                    ),
                    top: this.widget.viewModel.pixelPositions[i].top,
                    left: this.widget.viewModel.pixelPositions[i].left,
                    bottom: this.widget.viewModel.pixelPositions[i].bottom,
                    right: this.widget.viewModel.pixelPositions[i].right
                    ));*/
            }
            return allMark;
        }
        public void test()
        {
            //yield return null;
            this.widget.actionModel.markPositionUpdate(true);
        }
        public override Widget build(BuildContext context)
        {
            return new Container(
                padding:EdgeInsets.zero,
                //constraints: new BoxConstraints(minWidth:2000,minHeight:2000),
                height: 2000,
                width: 1000,
                child: new GestureDetector(
                    child: new Container(
                        padding: EdgeInsets.zero,
                        height: 1000, 
                        width: 2000, 
                        alignment:Unity.UIWidgets.painting.Alignment.center,
                        color: TapOn.Constants.CColors.Transparent,
                        child: new Stack(
                            children: _buildMark()
                            )
                        ),
                    onTapDown: detail => 
                    {
                        Vector2 t = new Vector2(detail.globalPosition.dx, detail.globalPosition.dy);
                        this.widget.actionModel.selectMark(t);
                    },
                    onPanStart:detail =>
                    {
                        //this.widget.actionModel.markPositionUpdate(false);
                    },
                    onPanEnd: async detail =>
                    {
                        QueryCallbackData<Marks> data = await BmobApi.queryFuzztMarksAsync(MapApi.map.GetCoordinate(), 3);

                        Debug.LogError(2);
                        List<Mark> marks = new List<Mark>();
                        foreach (var mark in data.results)
                            marks.Add(new Mark { coordinate = new Coordinate(mark.coordinate.Latitude.Get(), mark.coordinate.Longitude.Get()), id = mark.objectId, date = mark.upLoadTime, filePath = mark.snapShot.url });
                        this.widget.actionModel.addMarkJustLoading(marks);
                        this.widget.actionModel.changeMark();
                        //MapApi.mapEnd.velocity_x = detail.velocity.pixelsPerSecond.dx;
                        //MapApi.mapEnd.velocity_y = detail.velocity.pixelsPerSecond.dy;
                        //this.widget.actionModel.markPositionUpdate(true);
                        //this.widget.actionModel.loadMark();
                    },
                    onPanUpdate: details =>
                    {
                        this.widget.actionModel.mapMoveOffsetY(details.delta.dy);
                        this.widget.actionModel.mapMoveOffsetX(-details.delta.dx);
                        this.widget.actionModel.moveMap();
                    },
                    onScaleUpdate: details =>
                    {
                        this.widget.actionModel.mapZoom(details.scale);
                        this.widget.actionModel.zoomMap();
                    }
                )
            );
        }

        public Widget scaffold()
        {
            List<BottomNavigationBarItem> t = new List<BottomNavigationBarItem>
            {
                new BottomNavigationBarItem(icon: new Icon(icon: MyIcons.mark_icon, color: CColors.Black)),
                new BottomNavigationBarItem(icon: new Icon(icon: MyIcons.mark_icon, color: CColors.Black))
            };
            return new Scaffold(
                backgroundColor: CColors.Transparent,
                appBar: new AppBar(title: new Text("TapOn")),
                body: new Container(decoration: new BoxDecoration(color: CColors.Transparent)),
                bottomNavigationBar: new Container(
                    height: 50,
                    color: Colors.blue,
                    child: new Center(
                        child: new BottomNavigationBar(
                            type: BottomNavigationBarType.shifting,
                            // type: BottomNavigationBarType.fix,
                            items: new List<BottomNavigationBarItem> {
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.mark_icon, size: 30),
                                    title: new Text("Work"),
                                    activeIcon: new Icon(icon: MyIcons.mark_icon, size: 50),
                                    backgroundColor: Colors.blue
                                ),
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.mark_icon, size: 30),
                                    title: new Text("Home"),
                                    activeIcon: new Icon(icon: MyIcons.mark_icon, size: 50),
                                    backgroundColor: Colors.blue
                                ),
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.mark_icon, size: 30),
                                    title: new Text("Shop"),
                                    activeIcon: new Icon(icon: MyIcons.mark_icon, size: 50),
                                    backgroundColor: Colors.blue
                                ),
                                new BottomNavigationBarItem(
                                    icon: new Icon(icon: MyIcons.mark_icon, size: 30),
                                    title: new Text("School"),
                                    activeIcon: new Icon(icon: MyIcons.mark_icon, size: 50),
                                    backgroundColor: Colors.blue
                                ),
                            },
                            currentIndex: this._currentIndex,
                            onTap: (value) => { this.setState(() => { this._currentIndex = value; }); }
                        )
                    )
                )
            );
        }
    }
}
