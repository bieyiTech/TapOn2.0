using AREffect;
using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using Unity.UIWidgets.async;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.AREffects
{
    public class ARbase : MonoBehaviour
    {

        public GameObject arEffect;
        public GameObject arDisplay;

        public PropsController dragger;

        public AREffectManager arEffectManager;

        public CreateEditMapController CreateEdit;
        public PreviewEditController PreviewEdit;

        public void StartDrag(Prop prop)
        {
            dragger.StartCreate(prop);
        }

        public bool GetCanPlaceState()
        {
            return false;
        }

        public void StopDrag()
        {
            dragger.StopCreate();
        }

        public void SaveEdit()
        {
            CreateEdit.SaveEdit();
        }

        public void SetAREffectState(bool state)
        {
            arEffect.SetActive(state);
        }

        public void SetARDisplayState(bool state)
        {
            arDisplay.SetActive(state);
        }

        public void DisableAREffect()
        {
            arEffectManager.CreateAndEditMapEnd();
        }

        public void EnableAREffect()
        {
            arEffectManager.CreateAndEditMap();
        }

        public void PreviewMap(Mark mark, BuildContext context)
        {
            using (Unity.UIWidgets.widgets.WindowProvider.of(context).getScope())
            {
                Window.instance.startCoroutine(arEffectManager.PreviewMap(mark));
            }
        }

        public void EditMap()
        {
            arEffectManager.EditMap();
        }


        public void SaveEdit(BuildContext context)
        {
            using (Unity.UIWidgets.widgets.WindowProvider.of(context).getScope())
            {
                Window.instance.startCoroutine(CreateEdit.SaveEdit());
            }
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
