using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TapOn.Utils
{
    public static class TapOnUtils
    {
        public static IEnumerator WaitSomeTime(Action before = null, Action after = null, float time = 0.1f)
        {
            if (before != null) before();
            yield return new Unity.UIWidgets.async.UIWidgetsWaitForSeconds(time);
            if (after != null) after();
        }

        public static IEnumerator getWWW(string url, Action<UnityWebRequest> after)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();
            after(webRequest);
        }

        public static IEnumerator upLoadFile(string fileName, string contentType, string localPath, Action<UnityWebRequest> after)
        {
            string url = "https://api.bmob.cn/2/files/" + fileName;
            UnityWebRequest wr = new UnityWebRequest(url, "POST");
            wr.SetRequestHeader("X-Bmob-Application-Id", "694024c993688a00b5707fba73ab8551");
            wr.SetRequestHeader("X-Bmob-REST-API-Key", "60ad19f4362c54aa3da42a41282cb369");
            wr.SetRequestHeader("Content-Type", contentType);
            wr.uploadHandler = new UploadHandlerFile(localPath);
            wr.downloadHandler = new DownloadHandlerBuffer();
            yield return wr.SendWebRequest();
            if(wr.isHttpError || wr.isNetworkError)
            {
                Debug.LogError("FileUploadError: " + wr.error + "\nreturn: " + wr.downloadHandler.text);
            }
            else
            {
                after(wr);
            }
        }

        public static IEnumerator upLoadFile(string fileName, string contentType, byte[] fileContent, Action<UnityWebRequest> after)
        {
            string url = "https://api.bmob.cn/2/files/" + fileName;
            UnityWebRequest wr = new UnityWebRequest(url, "POST");
            wr.SetRequestHeader("X-Bmob-Application-Id", "694024c993688a00b5707fba73ab8551");
            wr.SetRequestHeader("X-Bmob-REST-API-Key", "60ad19f4362c54aa3da42a41282cb369");
            wr.SetRequestHeader("Content-Type", contentType);
            wr.uploadHandler = new UploadHandlerRaw(fileContent);
            wr.downloadHandler = new DownloadHandlerBuffer();
            yield return wr.SendWebRequest();
            if (wr.isHttpError || wr.isNetworkError)
            {
                Debug.LogError("FileUploadError: " + wr.error + "\nreturn: " + wr.downloadHandler.text);
            }
            else
            {
                after(wr);
            }
        }

        public static Restful_FileUpLoadCallBack fileUpLoadCallBackfromJson(string jsonText)
        {
            return JsonUtility.FromJson<Restful_FileUpLoadCallBack>(jsonText);
        }
    }

    public class Restful_FileUpLoadCallBack
    {
        public string filename;
        public string url;
        public string cdnname;
    }
}
