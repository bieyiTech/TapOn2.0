//================================================================================================================================
//
//  Copyright (c) 2015-2020 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using UnityEngine;
using System.Collections;

namespace AREffect
{
    public class OneShot : MonoBehaviour
    {
        private bool mirror;
        private Action<Texture2D> callback;
        private bool capturing;
        
        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination);
            if (!capturing) { return; }

            var destTexture = new RenderTexture(Screen.width, Screen.height, 0);
            if (mirror)
            {
                var mat = Instantiate(Resources.Load<Material>("Sample_MirrorTexture"));
                mat.mainTexture = source;
                Graphics.Blit(null, destTexture, mat);
            }
            else
            {
                Graphics.Blit(source, destTexture);
            }

            RenderTexture.active = destTexture;
            var texture = new Texture2D(200, 200, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(Screen.width-100, Screen.height-100, 200, 200), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            Destroy(destTexture);

            callback(texture);
            CreateEditMapController.SnapShotDone = true;
            Destroy(this);
        }

        public void Shot(bool mirror, Action<Texture2D> callback)
        {
            if (callback == null) { return; }
            this.mirror = mirror;
            this.callback = callback;
            capturing = true;
        }
    }
}
