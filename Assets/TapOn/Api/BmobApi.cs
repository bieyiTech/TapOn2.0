using AREffect;
using cn.bmob.api;
using cn.bmob.io;
using cn.bmob.response;
using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TapOn.Constants;
using TapOn.Models;
using TapOn.Models.DataModels;
using TapOn.Utils;
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.async;
using Unity.UIWidgets.ui;
using UnityEngine;

namespace TapOn.Api
{
    public class BmobApi
    {
        public static BmobUnity Bmob { get { return Globals.instance.bmob; } }

        public static async Task<QueryCallbackData<Mark>> queryFuzztMarksAsync(Coordinate coodinate, int limit)
        {
            BmobQuery query = new BmobQuery();
            query.WhereNear("position", new BmobGeoPoint(coodinate.latitude, coodinate.lontitude));
            query.Limit(limit);
            return await Bmob.FindTaskAsync<Mark>(Mark.table_name, query);
        }

        public static async Task<QueryCallbackData<BmobModel>> queryAllModelsMessage()
        {
            BmobQuery query = new BmobQuery();
            query.Limit(100);
            return await Bmob.FindTaskAsync<BmobModel>(BmobModel.table_name, query);
        }

        public static Task<List<Prop>> getPropsInMark(Mark mark)
        {
            return Task.Run(
                async () =>
                {
                    BmobQuery query = new BmobQuery();
                    query.WhereEqualTo("mark", new BmobPointer<Mark>(mark));
                    QueryCallbackData<Prop> data = await Bmob.FindTaskAsync<Prop>(Prop.table_name, query);
                    List<Prop> props = new List<Prop>();
                    foreach(Prop prop in data.results)
                    {
                        /*Vector3 pos = new Vector3((float)prop.pos_x.Get(), (float)prop.pos_y.Get(), (float)prop.pos_z.Get());
                        Vector4 rot = new Vector4((float)prop.rot_x.Get(), (float)prop.rot_y.Get(), (float)prop.rot_z.Get(), (float)prop.rot_w.Get());
                        Vector3 sca = new Vector3((float)prop.scale_x.Get(), (float)prop.scale_y.Get(), (float)prop.scale_z.Get());
                        if (prop.type.Get() == 0)
                            props.Add(new TextProp { text = prop.text, type = 0, position = pos, rotation = rot, scale = sca, });
                        if (prop.type.Get() == 1)
                            props.Add(new ImageProp { fileName = prop.texture.filename, url = prop.texture.url, type = 1, position = pos, rotation = rot, scale = sca, });
                        if (prop.type.Get() == 2)
                            props.Add(new VideoProp { fileName = prop.video.filename, url = prop.video.url, type = 2 position = pos, rotation = rot, scale = sca, });
                        if (prop.type.Get() == 3)
                            props.Add(new ModelProp { fileName = prop.model.reference.model.filename, url = prop.model.reference.model.url, type = 3, position = pos, rotation = rot, scale = sca, });*/
                        props.Add(prop);
                    }
                    return props;
                });
        }

        public static IEnumerator getMetaFile(BmobFile metaFile, Action<List<GameObject>> after)
        {
            MapMeta meta = null;
            yield return TapOnUtils.downloadFile(
                metaFile.url,
                wr => 
                {
                    meta = JsonUtility.FromJson<MapMeta>(wr.downloadHandler.text);
                });
            List<GameObject> objects = new List<GameObject>();
            List<IEnumerator> tasks = new List<IEnumerator>();
            foreach (MapMeta.PropInfo propInfo in meta.Props)
            {
                if (propInfo.type == MapMeta.PropType.Text)
                {
                    GameObject instance = GameObject.Instantiate(Globals.instance.templetes[0]);
                    instance.tag = "word";
                    instance.SetActive(false);
                    TextMesh tm = instance.GetComponentInChildren<TextMesh>();
                    tm.text = propInfo.text;
                    instance.transform.position = new Vector3(propInfo.Position[0], propInfo.Position[1], propInfo.Position[2]);
                    instance.transform.rotation = new Quaternion(propInfo.Rotation[0], propInfo.Rotation[1], propInfo.Rotation[2], propInfo.Rotation[3]);
                    instance.transform.localScale = new Vector3(propInfo.Scale[0], propInfo.Scale[1], propInfo.Scale[2]);
                    objects.Add(instance);
                }
                if (propInfo.type == MapMeta.PropType.Texture)
                {
                    GameObject instance = GameObject.Instantiate(Globals.instance.templetes[1]);
                    instance.tag = "texture";
                    instance.SetActive(false);
                    Renderer rd = instance.GetComponentInChildren<Renderer>();
                    instance.transform.position = new Vector3(propInfo.Position[0], propInfo.Position[1], propInfo.Position[2]);
                    instance.transform.rotation = new Quaternion(propInfo.Rotation[0], propInfo.Rotation[1], propInfo.Rotation[2], propInfo.Rotation[3]);
                    instance.transform.localScale = new Vector3(propInfo.Scale[0], propInfo.Scale[1], propInfo.Scale[2]);
                    objects.Add(instance);

                    IEnumerator downloadTexture = TapOnUtils.downloadFile(
                        propInfo.infoUrl,
                        wr_dt =>
                        {
                            Texture2D texture = new Texture2D(10, 10);
                            texture.LoadImage(wr_dt.downloadHandler.data);
                            rd.material.mainTexture = texture;
                        });
                    tasks.Add(downloadTexture);
                }
            }
            foreach (IEnumerator t in tasks)
            {
                yield return t;
            }
            if (after != null)
            {
                after(objects);
            }
        }

        /*public static IEnumerator getAllProps()
        {

        }*/
        public static async void addMarktoServer(Mark mark)
        {
            CreateCallbackData callback_mark = await Bmob.CreateTaskAsync(Mark.table_name, mark);
            if (callback_mark == null || callback_mark.objectId == null || callback_mark.objectId.Length == 0)
            {
                Debug.LogError("BmobRrror: Mark hasn't upload!");
                return;
            }
            Globals.instance.uploading = false;
            ((Unity.UIWidgets.widgets.Element)Globals.instance.nowContext).markNeedsBuild();
            
            using (Unity.UIWidgets.widgets.WindowProvider.of(Globals.instance.nowContext).getScope())
            {
                Window.instance.startCoroutine(
                TapOnUtils.WaitSomeTime(
                    time: 0.5f,
                    after: () =>
                    {
                        Debug.Log("in back home!");
                        Globals.instance.returnHome(() => { Globals.instance.returnMap(); });
                    })
                );
                Window.instance.startCoroutine(
                    TapOnUtils.upLoadFile(
                        "img_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".jpg",
                        "application/x-jpg",
                        mark.snapShot_byte,
                        wr =>
                        {
                            Restful_FileUpLoadCallBack t = TapOnUtils.fileUpLoadCallBackfromJson(wr.downloadHandler.text);
                            BmobFile bf = new BmobFile { filename = t.filename, url = t.url, group = t.cdnname };
                            Mark m = new Mark { snapShot = bf };
                            Bmob.Update(
                                Mark.table_name,
                                callback_mark.objectId,
                                m,
                                (resp, exception) =>
                                {
                                    if (exception != null)
                                    {
                                        Debug.Log("修改失败, 失败原因为： " + exception.Message);
                                        return;
                                    }

                                    Debug.Log("修改成功, @" + resp.updatedAt);
                                });
                        }));
                Window.instance.startCoroutine(
                    TapOnUtils.upLoadFile(
                        "Props_" + DateTime.Now.ToString("yyyy-MM-,") + ".json",
                        "application/json",
                        mark.meta_byte,
                        wr =>
                        {
                            Restful_FileUpLoadCallBack t = TapOnUtils.fileUpLoadCallBackfromJson(wr.downloadHandler.text);
                            BmobFile bf = new BmobFile { filename = t.filename, url = t.url, group = t.cdnname };
                            Mark m = new Mark { metaFile = bf };
                            Bmob.Update(
                                Mark.table_name,
                                callback_mark.objectId,
                                m,
                                (resp, exception) =>
                                {
                                    if (exception != null)
                                    {
                                        Debug.Log("修改失败, 失败原因为： " + exception.Message);
                                        return;
                                    }

                                    Debug.Log("修改成功, @" + resp.updatedAt);
                                });
                        }));
            }
        }

        /*public static IPromise<List<Mark>> queryFuzzyMarks(Coordinate coodinate, int limit)
        {
            BmobQuery query = new BmobQuery();
            query.WhereNear("position", new BmobGeoPoint(coodinate.latitude, coodinate.lontitude));
            query.Limit(limit);
            Debug.LogError("1");
            List<Mark> marks = new List<Mark>();
            Promise<List<Mark>> p =  new Promise<List<Mark>>(async (resolve, reject) =>
            {
                Debug.LogError(2);
                QueryCallbackData<BmobMark> data = await Bmob.FindTaskAsync<BmobMark>(BmobMark.table_name, query);
                Debug.LogError(333);
                Debug.LogError(data.results.Count);
                
                foreach (var mark in data.results)
                {
                    marks.Add(new Mark { coordinate = new Coordinate(mark.coordinate.Latitude.Get(), mark.coordinate.Longitude.Get()), id = mark.objectId });
                }
                Debug.LogError(323);
                resolve(marks);
                Debug.LogError(334);
                //Window.instance.startCoroutine(getMark(resolve, reject, query));
            });
            return p;
            //return promise;
            //Bmob.CreateTaskAsync()
            //query.
        }*/

        /*public static IPromise<List<Mark>> queryAccurateMarks(Coordinate ws, Coordinate ne)
        {
            
            BmobQuery query = new BmobQuery();
            query.WhereWithinGeoBox("position", new BmobGeoPoint(ws.latitude, ws.lontitude), new BmobGeoPoint(ne.latitude, ne.lontitude));
            Promise<List<Mark>> promise = new Promise<List<Mark>>();
            Window.instance.startCoroutine(getMark(promise, query));
            Debug.LogError(4);
            return promise;
            //Bmob.CreateTaskAsync()
            //query.
        }*/

        /*public static IEnumerator getMark(Action<List<Mark>> resolve, Action<Exception> reject, BmobQuery query)
        {
            List<Mark> marks = new List<Mark>();
            Exception ex = new Exception();
            Bmob.Find<BmobMark>(BmobMark.table_name, query, (resp, exception) =>
            {
                if (exception != null)
                {
                    Debug.LogError(5);
                    reject(exception);
                    return;
                }

                List<BmobMark> list = resp.results;
                Debug.LogError(6);
                Debug.LogError("list: " + list.Count);
                foreach (var mark in list)
                {
                    marks.Add(new Mark { coordinate = new Coordinate(mark.coordinate.Latitude.Get(), mark.coordinate.Longitude.Get()), id = mark.objectId });
                }
            });
            yield return null;

            resolve(marks);
            Debug.LogError("2");
        }*/

        /*public static IPromise<string> createMark(Coordinate coordinate)
        {
            Promise<string> promise = new Promise<string>();
            BmobMark m = new BmobMark { coordinate = new BmobGeoPoint(coordinate.latitude, coordinate.lontitude) };
            Window.instance.startCoroutine(addMark(promise, m));
            return promise;
        }

        public static IEnumerator addMark(Promise<string> promise, BmobMark m)
        {
            yield return null;
            Bmob.Create(BmobMark.table_name, m, (resp, exception) =>
            {
                if (exception != null)
                {
                    promise.Reject(exception);
                    return;
                }
                promise.Resolve(value: resp.createdAt);
            });
            
        }*/

    }
}
