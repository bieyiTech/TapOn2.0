using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

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
        WWW www = new WWW(url);
#pragma warning restore CS0618 // 类型或成员已过时
        yield return www;

        byte[] bytes = www.texture.EncodeToJPG();
        string path = PathForFile(fileName, dic);

        File.WriteAllBytes(path, bytes);

        Sprite tempSp = Sprite.Create(www.texture, new Rect(0, 0, 200, 200), new Vector2(0.5f, 0.5f));

        // Add Sprite GameObject
        GameObject tempSprite = new GameObject("Sprite");
        tempSprite.transform.parent = Mark.transform;
        tempSprite.transform.localPosition = new Vector3(0, 0.45f, 0);
        tempSprite.transform.localRotation = Quaternion.identity;
        tempSprite.transform.localScale = Vector3.one;

        // Add Sprite Texture
        SpriteRenderer spriteRenderer = tempSprite.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tempSp;
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        // Add Sprite Mask
        tempSprite.AddComponent<SpriteMask>().sprite = spriteMaskCircle;

    }

    

    /// <summary>
    /// 在不同平台保存
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    private string PathForFile(string filename, string dic)
    {

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, "Documents");
            path = Path.Combine(path, dic);
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, dic);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, dic);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
    }
    

}
