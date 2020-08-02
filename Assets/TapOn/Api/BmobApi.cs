﻿using cn.bmob.api;
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

        public static Task<bool> addMarktoServer(Mark mark)
        {
            return Task.Run(
                async () =>
                {
                    Debug.Log("snapShot_byte Length: " + mark.snapShot_byte.Length);
                    UploadCallbackData ud = await Bmob.FileUploadTaskAsync(new BmobLocalFile(mark.snapShot_byte, "Img_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".jpg"));
                    if (ud.filename == null || ud.filename.Length == 0)
                        return false;
                    Debug.Log(ud.url);

                    mark.snapShot = ud;
                    UploadCallbackData um = await Bmob.FileUploadTaskAsync(new BmobLocalFile(mark.meta_byte, "Map_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".meta"));
                    if (um.filename == null || um.filename.Length == 0)
                        return false;
                    mark.meta = um;

                    CreateCallbackData callback_mark = await Bmob.CreateTaskAsync(Mark.table_name, mark);
                    if (callback_mark == null || callback_mark.objectId == null || callback_mark.objectId.Length == 0)
                        return false;

                    //foreach(Prop prop in mark.props)
                    //{
                    //    prop.mark = mark;
                    //    CreateCallbackData callback_prop = await Bmob.CreateTaskAsync(Prop.table_name, prop);
                    //}

                    return true;
                });
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
