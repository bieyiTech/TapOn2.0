using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AREffect
{
    public class PreviewEidtController : MonoBehaviour
    {
        public PropsController PropDragger;

        private MapSession mapSession;
        private MapSession.MapData mapData;

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


    }
}