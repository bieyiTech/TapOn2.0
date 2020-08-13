using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AREffect
{
    public class UrlManager : MonoBehaviour
    {
        private string infoUrl;
        private string infoFileName;
        private MapMeta.PropType infoType;
        private bool loadUrl = false;

        private static Vector3 imageTF = Vector3.one;
        private static float ratio = 1.0f;
        private static bool saveImgTf = true;


        public void SetUrl(string url, string fileName, MapMeta.PropType type)
        {
            infoUrl = url;
            infoFileName = fileName;
            infoType = type;
            loadUrl = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(loadUrl && this.gameObject.activeSelf)
            {
                switch (infoType)
                {
                    case MapMeta.PropType.Texture:
                        StartCoroutine(LoadTexture(infoUrl, infoFileName));
                        break;
                    case MapMeta.PropType.Video:
                        break;
                    case MapMeta.PropType.Model:
                        break;
                }
                loadUrl = false;
            }
        }

        private IEnumerator LoadTexture(string url, string fileName)
        {
            //Debug.Log("LoadTexture");
            // 判定texture是否本地存在

#pragma warning disable CS0618 // 类型或成员已过时
            WWW www;
            if(MapMetaManager.isTextureInLocal(fileName))
            {
                www = new WWW("file://" + MapMetaManager.PathForFile(fileName, "picture"));
                //Debug.Log("prop: get From local");
            }
            else
            {
                www = new WWW(url);
                //Debug.Log("prop: get From url");
            }
#pragma warning restore CS0618 // 类型或成员已过时
            yield return www;

            this.tag = "texture";
            Transform tf = this.transform.Find("Cube");
            if (saveImgTf)
            {
                saveImgTf = false;
                imageTF = this.transform.Find("Cube").localScale;
                ratio = imageTF.y * 1.0f / imageTF.x;
            }
            float ratio_tex = www.texture.height * 1.0f / www.texture.width;
            //Debug.Log("ratio: " + ratio + " ratio_text: " + ratio_tex);

            tf.localScale = ratio_tex > ratio ?
                new Vector3(ratio / ratio_tex * imageTF.x, imageTF.y, imageTF.z) :
                new Vector3(imageTF.x, ratio_tex / ratio * imageTF.y, imageTF.z);

            Renderer rd = this.GetComponentInChildren<Renderer>();
            rd.material.mainTexture = www.texture;
            // 存储texture 到本地。
            MapMetaManager.SaveTextureToLocal(www.texture, fileName);


        }
    }
}