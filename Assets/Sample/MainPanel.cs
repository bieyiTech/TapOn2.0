using Models.Redux;
using Models.State;
using Screens;
using TapOn.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.widgets;
using UnityEngine;

public class MainPanel : UIWidgetsPanel
{
    static PageRouteFactory pageRouteBuilder {
            get {
                return (settings, builder) =>
                    new PageRouteBuilder(
                        settings: settings,
                        (context, animation, secondaryAnimation) => builder(context)
                    );
            }
        }
    protected override Widget createWidget()
    {
        return new StoreProvider<AppState>(
                store: StoreProvider.store,
                new WidgetsApp(
                    home: new newScreenConnector(),
                    pageRouteBuilder: pageRouteBuilder
                )
            );
    }
}
