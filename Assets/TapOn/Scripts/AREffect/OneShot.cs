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
        private int ImageSize = 200;

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
            var sideLength = Screen.width > Screen.height ? Screen.height : Screen.width;
            var textureBefore = new Texture2D(sideLength, sideLength, TextureFormat.RGB24, false);
            textureBefore.ReadPixels(new Rect((Screen.width- sideLength)/2, (Screen.height- sideLength)/2, sideLength, sideLength), 0, 0);
            var texture = ScaleTexture(textureBefore, ImageSize, ImageSize);
            texture.Apply();
            RenderTexture.active = null;
            Destroy(destTexture);

            callback(texture);
            CreateEditMapController.SnapShotDone = true;
            Destroy(this);
        }

        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
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
