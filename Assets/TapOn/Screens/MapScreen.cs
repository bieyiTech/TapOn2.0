using cn.bmob.response;
using RSG;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                        zoomLevel = state.mapState.zoomLevel,
                        marks = state.mapState.marks,
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
                        changeZoomLevel = z =>
                            dispatcher.dispatch(new ChangeMapZoomLevelAction { zoomLevel = z }),
                        mapZoom = s =>
                            dispatcher.dispatch(new MapZoomAction { scale = s }),
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
        public int _currentIndex = 0;

        public override void initState()
        {
            base.initState();
            update();
            //updateMarks();
        }

        private async void updateMarks()
        {
            QueryCallbackData<Marks> data = await BmobApi.queryFuzztMarksAsync(Prefabs.instance.mapController.GetCoordinate(), 3);
            List<Mark> marks = new List<Mark>();
            foreach (var mark in data.results)
            {
                marks.Add(new Mark { coordinate = new Coordinate(mark.coordinate.Latitude.Get(), mark.coordinate.Longitude.Get()),
                    id = mark.objectId,
                    date = mark.upLoadTime,
                    url = mark.snapShot.url,
                    fileName = mark.snapShot.filename
                });

            }
            this.widget.actionModel.addMarkJustLoading(marks);
            this.widget.actionModel.changeMark();
        }

        private Task<bool> waitForMapController()
        {
            return Task.Run(
                () =>
                {
                    for (int i = 0; i < 100000; i++)
                        if(Prefabs.instance.mapController!=null)
                        {
                            Debug.Log(i);
                            return true;
                        }
                    return false;
                });
        }

        private async void update()
        {
            bool t = await waitForMapController();
            if (t) updateMarks();
        }
        public override Widget build(BuildContext context)
        {
            return new Container(
                padding:EdgeInsets.zero,
                height: 2000,
                width: 1000,
                child: new GestureDetector(
                    child: new Container(
                        padding: EdgeInsets.zero,
                        height: 1000, 
                        width: 2000, 
                        alignment:Alignment.center,
                        color: CColors.Transparent
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
                        Debug.Log("velocity: " + detail.velocity.pixelsPerSecond.dx);
                        QueryCallbackData<Marks> data = await BmobApi.queryFuzztMarksAsync(MapApi.map.GetCoordinate(), 3);

                        List<Mark> marks = new List<Mark>();
                        foreach (var mark in data.results)
                        {
                            marks.Add(new Mark
                            {
                                coordinate = new Coordinate(mark.coordinate.Latitude.Get(), mark.coordinate.Longitude.Get()),
                                id = mark.objectId,
                                date = mark.upLoadTime,
                                url = mark.snapShot.url,
                                fileName = mark.snapShot.filename
                            });
                            Debug.Log("mark.upLoadTime: " + mark.upLoadTime);
                            Debug.Log("mark.snapShot.url: " + mark.snapShot.url);
                        }
                            
                        this.widget.actionModel.addMarkJustLoading(marks);
                        this.widget.actionModel.changeMark();
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
                        if ((int)(this.widget.viewModel.zoomLevel) != (int)(Prefabs.instance.mapController.GetZoomLevel()))
                        {
                            this.widget.actionModel.changeZoomLevel(Prefabs.instance.mapController.GetZoomLevel());
                            Debug.Log("zoomlevel change!");
                        }
                    },
                    onScaleStart: detail =>
                    {
                        this.widget.actionModel.changeZoomLevel(Prefabs.instance.mapController.GetZoomLevel());
                    },
                    onScaleEnd: detail =>
                    {
                        this.widget.actionModel.mapZoom(1);
                    }
                )
            );
        }
        
    }
}
