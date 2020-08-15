using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using AREffect;

public class SnapShotLoad : MonoBehaviour
{
    public Sprite spriteMaskCircle;

    /// <summary>
    /// 从url下载图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="filePath"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public IEnumerator LoadSnapShot(string url, string fileName, string dic, GameObject Mark)
    {
#pragma warning disable CS0618 // 类型或成员已过时
        WWW www;
        if (MapMetaManager.isTextureInLocal(fileName))
        {
            www = new WWW("file://" + MapMetaManager.PathForFile(fileName, "picture"));
        }
        else
        {
            www = new WWW(url);
        }
#pragma warning restore CS0618 // 类型或成员已过时
        yield return www;

        MapMetaManager.SaveTextureToLocal(www.texture, fileName);

        Sprite tempSp = Sprite.Create(www.texture, new Rect(0, 0, 200, 200), new Vector2(0.5f, 0.5f));

        // Add Sprite GameObject
        GameObject tempSprite = new GameObject("Sprite");
        tempSprite.transform.parent = Mark.transform;
        tempSprite.transform.localPosition = new Vector3(0, 0.45f, 0);
        tempSprite.transform.localRotation = Quaternion.identity;
        tempSprite.transform.localScale = Vector3.one;
        tempSprite.layer = 256;

        // Add Sprite Texture
        SpriteRenderer spriteRenderer = tempSprite.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tempSp;
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        // Add Sprite Mask
        tempSprite.AddComponent<SpriteMask>().sprite = spriteMaskCircle;

    }
    
}
