using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AREffect
{
    public class PreviewEditController : MonoBehaviour
    {
        public PropsController PropDragger;
        public MapSession.MapData mapData;

        private MapSession mapSession;

        private void Awake()
        {
            PropDragger.CreateObject += (gameObj) =>
            {
                if (gameObj)
                {
                    gameObj.transform.parent = mapData.Controller.transform;
                    mapData.Props.Add(gameObj);
                }
            };
            PropDragger.DeleteObject += (gameObj) =>
            {
                if (gameObj)
                {
                    mapData.Props.Remove(gameObj);
                }
            };
        }

        public void SetMapSession(MapSession session)
        {
            mapSession = session;
            PropDragger.SetMapSession(session);
            mapData = mapSession.Maps[0];
        }


    }
}