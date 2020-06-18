using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TapOn.Models.DataModels;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Components
{
    public class ModelGrid : StatefulWidget
    {
        public readonly Decoration decoration;
        public readonly int columnNum = 2;
        public readonly List<MyIcon> allIcon;
        public ModelGrid(
            Key key = null,
            Decoration decoration = null,
            int columnNum = 2,
            List<MyIcon> allIcon = null
        ): base(key: key)
        {
            this.decoration = decoration;
            this.columnNum = columnNum;
            this.allIcon = allIcon;
        }

        public override State createState()
        {
            return new _ModelGrid();
        }
    }

    class _ModelGrid : State<ModelGrid>
    {
        private Widget _line(List<MyIcon> icons)
        {
            var children_list = icons.Select((item) => {
                return (Widget)new Container(
                    height: 200,
                    padding: EdgeInsets.fromLTRB(4, 0, 4, 0),
                    child: new Column(
                        crossAxisAlignment: CrossAxisAlignment.center,
                        children: new List<Widget>
                        {
                            item.icon,
                            new Text(
                                item.text,
                                style: new TextStyle(
                                fontSize: 14,
                                height: 1.4f
                                )
                            )
                        }
                    )
                );
            });
            return new Flexible(
                   child: new ListView(
                       //physics: new AlwaysScrollableScrollPhysics(),
                       children: children_list.ToList()
                   )
               );
        }
        private List<Widget> _branch()
        {
            List<Widget> listAfterBranch = new List<Widget>();
            for(int i = 0;i < this.widget.allIcon.Count / this.widget.columnNum + 1; i ++)
            {
                List<MyIcon> icons = new List<MyIcon>();
                for(int j = i;j < i+this.widget.columnNum;j ++)
                {
                    icons.Add(this.widget.allIcon[j]);
                }
                listAfterBranch.Add(_line(icons));
            }
            return listAfterBranch;
        }
        public override Widget build(BuildContext context)
        {
            return new Container(
               child: new Column(
                    children: _branch()
                    )
                );
        }
    }
}
