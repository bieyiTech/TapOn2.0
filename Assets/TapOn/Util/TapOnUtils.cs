using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public static IEnumerator downloadFile(string url, Action<UnityWebRequest> after)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError("FileUploadError: " + webRequest.error + "\nreturn: " + webRequest.downloadHandler.text);
            }
            else
            {
                after(webRequest);
            }
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

        public static IEnumerator downloadModel(string url, string filename, Action<float> progressSolve, Action<UnityWebRequest> after)
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            request.SendWebRequest();
 
            while(!request.isDone)
            {
                
                if (progressSolve != null)
                    progressSolve(request.downloadProgress);
                yield return null;
                //yield return WaitSomeTime(time: 0.01f);
            }
            if(request.isDone)
            {
                if (progressSolve != null)
                    progressSolve(1);
            }
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("FileUploadError: " + request.error + "\nreturn: " + request.downloadHandler.text);
            }
            else
            {
                Stream sw;
                FileInfo t = new FileInfo(Application.persistentDataPath + "//Model//" + filename);
                Debug.Log(filename);
                if (after != null)
                    after(request);
            }
        }

        public static UnityEngine.LocationInfo nowLocation
        { get { return Input.location.lastData; } }

        public static IEnumerator startGPS()
        {
            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("GPS服务未启用");
            }

            Input.location.Start(10.0f, 10.0f);

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                Debug.Log("GPS服务启动超时");
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("GPS服务无法确定位置");
            }
            else
            {
                yield return new WaitForSeconds(100);
            }
        }

        public static void stopGPS()
        {
            Input.location.Stop();
        }
    }

    public class Restful_FileUpLoadCallBack
    {
        public string filename;
        public string url;
        public string cdnname;
    }
}
