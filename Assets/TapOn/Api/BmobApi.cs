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
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.async;
using Unity.UIWidgets.ui;
using UnityEngine;

namespace TapOn.Api
{
    public class BmobApi
    {
        public static BmobUnity Bmob { get { return Prefabs.instance.bmob; } }

        public static async Task<QueryCallbackData<Marks>> queryFuzztMarksAsync(Coordinate coodinate, int limit)
        {
            BmobQuery query = new BmobQuery();
            query.WhereNear("position", new BmobGeoPoint(coodinate.latitude, coodinate.lontitude));
            query.Limit(limit);
            return await Bmob.FindTaskAsync<Marks>(Marks.table_name, query);
        }

        public static async Task<QueryCallbackData<BmobModel>> queryAllModelsMessage()
        {
            BmobQuery query = new BmobQuery();
            query.Limit(100);
            return await Bmob.FindTaskAsync<BmobModel>(BmobModel.table_name, query);
        }

        public static IPromise<List<Mark>> queryFuzzyMarks(Coordinate coodinate, int limit)
        {
            BmobQuery query = new BmobQuery();
            query.WhereNear("position", new BmobGeoPoint(coodinate.latitude, coodinate.lontitude));
            query.Limit(limit);
            Debug.LogError("1");
            List<Mark> marks = new List<Mark>();
            Promise<List<Mark>> p =  new Promise<List<Mark>>(async (resolve, reject) =>
            {
                Debug.LogError(2);
                QueryCallbackData<Marks> data = await Bmob.FindTaskAsync<Marks>(Marks.table_name, query);
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
        }

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

        public static IEnumerator getMark(Action<List<Mark>> resolve, Action<Exception> reject, BmobQuery query)
        {
            List<Mark> marks = new List<Mark>();
            Exception ex = new Exception();
            Bmob.Find<Marks>(Marks.table_name, query, (resp, exception) =>
            {
                if (exception != null)
                {
                    Debug.LogError(5);
                    reject(exception);
                    return;
                }

                List<Marks> list = resp.results;
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
        }

        public static IPromise<string> createMark(Coordinate coordinate)
        {
            Promise<string> promise = new Promise<string>();
            Marks m = new Marks { coordinate = new BmobGeoPoint(coordinate.latitude, coordinate.lontitude) };
            Window.instance.startCoroutine(addMark(promise, m));
            return promise;
        }

        public static IEnumerator addMark(Promise<string> promise, Marks m)
        {
            yield return null;
            Bmob.Create(Marks.table_name, m, (resp, exception) =>
            {
                if (exception != null)
                {
                    promise.Reject(exception);
                    return;
                }
                promise.Resolve(value: resp.createdAt);
            });
            
        }

    }
}
