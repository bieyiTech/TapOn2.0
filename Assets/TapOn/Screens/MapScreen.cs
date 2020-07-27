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
using Unity.UIWidgets.animation;
using Unity.UIWidgets.async;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
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
                    return new MapScreenViewModel
                    {
                        zoomLevel = state.mapState.zoomLevel,
                        scale = state.mapState.scale,
                        scaleLastFrame = state.mapState.scaleLastFrame,
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
                        moveMap = () => dispatcher.dispatch<object>(Actions.moveMap()),
                        zoomMap = () => dispatcher.dispatch<object>(Actions.zoomMap()),
                        changeMark = () => dispatcher.dispatch<object>(Actions.changeMark()),
                        loadMark = () => dispatcher.dispatch<object>(Actions.loadMark()),
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

        AnimationController am_horizontal;
        AnimationController am_vertical;
        AnimationController am_scale;
        Animation<float> animation_horizontal;
        Animation<float> animation_vertical;
        Animation<float> animation_scale;

        float lastHorizontal = 0;
        float lastVertical = 0;

        float lastScale = 1;

        public override void initState()
        {
            base.initState();
            Globals.instance.models = new List<GameObject>();
            update();
        }

        private async void updateMarks()
        {
            QueryCallbackData<BmobMark> data = await BmobApi.queryFuzztMarksAsync(Globals.instance.mapController.GetCoordinate(), 3);
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

            }
            this.widget.actionModel.addMarkJustLoading(marks);
            this.widget.actionModel.changeMark();
        }

        private IEnumerator wait_500()
        {
            yield return new UIWidgetsWaitForSeconds(0.55f);
            updateMarks();
        }

        private Task<bool> waitForMapController()
        {
            return Task.Run(
                () =>
                {
                    for (int i = 0; i < 100000; i++)
                        if(Globals.instance.mapController!=null)
                        {
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

        private IPromise<object> showBottomSheet()
        {
            return BottomSheetUtils.showBottomSheet(
                context: context,
                builder: (context) =>
                {
                    return new BottomAppBar(
                        color: CColors.White,
                        child: new Container(width: 1000, height: 80, child: new Text("455"))
                        );
                })._completer;
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
                    onPanEnd: detail =>
                    {
                        if (am_horizontal != null) am_horizontal.dispose();
                        if (am_vertical != null) am_vertical.dispose();
                        am_horizontal = new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, 0, 0, 500));
                        am_vertical = new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, 0, 0, 500));
                        animation_horizontal = new FloatTween(0, -detail.velocity.pixelsPerSecond.dx * 0.1f).chain(new CurveTween(Curves.decelerate)).animate(am_horizontal);
                        animation_vertical = new FloatTween(0, detail.velocity.pixelsPerSecond.dy * 0.1f).chain(new CurveTween(Curves.decelerate)).animate(am_vertical);
                        animation_horizontal.addListener(() => 
                        {
                            MapApi.MoveMap(animation_horizontal.value - lastHorizontal, animation_vertical.value - lastVertical);
                            lastHorizontal = animation_horizontal.value;
                            lastVertical = animation_vertical.value;
                        });
                        animation_horizontal.addStatusListener(status => 
                        {
                            if (status == AnimationStatus.completed)
                            {
                                lastHorizontal = 0;
                                lastVertical = 0;
                            }
                        });
                        am_horizontal.forward();
                        am_vertical.forward();
                        Window.instance.startCoroutine(wait_500());
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
                        if ((int)(this.widget.viewModel.zoomLevel) != (int)(Globals.instance.mapController.GetZoomLevel()))
                        {
                            this.widget.actionModel.changeZoomLevel(Globals.instance.mapController.GetZoomLevel());
                            Debug.Log("zoomlevel change!");
                        }
                    },
                    onScaleStart: detail =>
                    {
                        this.widget.actionModel.changeZoomLevel(Globals.instance.mapController.GetZoomLevel());
                    },
                    onScaleEnd: detail =>
                    {
                        if (am_scale != null) am_scale.dispose();
                        am_scale = new AnimationController(vsync: this, duration: new System.TimeSpan(0, 0, 0, 0, 500));
                        animation_scale = new FloatTween(widget.viewModel.scale, widget.viewModel.scale + (widget.viewModel.scale - widget.viewModel.scaleLastFrame) * 0.3f).chain(new CurveTween(Curves.decelerate)).animate(am_scale);
                        lastScale = widget.viewModel.scale;
                        animation_scale.addListener(() =>
                        {
                            MapApi.ZoomMap(animation_scale.value, lastScale);
                            lastScale = animation_scale.value;
                        });
                        am_scale.forward();
                        this.widget.actionModel.mapZoom(1);
                    }
                )
            );
        }
        
    }
}
