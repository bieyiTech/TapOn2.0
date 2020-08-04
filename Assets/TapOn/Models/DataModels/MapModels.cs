using cn.bmob.io;
using System.Collections;
using System.Collections.Generic;
using TencentMap.CoordinateSystem;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace TapOn.Models.DataModels
{
    /// <summary>
    /// latitude and longitude, maybe defined in sdk
    /// </summary>
    /*public class Coordinate
    {
        double latitude;
        double longitude;
    }*/
    /// <summary>
    /// the user's mark on map
    /// </summary>
    public class Mark : BmobTable
    {
        public static string table_name = "Mark";

        public string MapId;
        public string MapName;
        
        public GameObject logoInstance;

        public BmobGeoPoint coordinate;
        //public string userId;

        public List<Prop> props;

        public BmobFile snapShot;
        public byte[] snapShot_byte;
        public BmobFile meta;
        public byte[] meta_byte;
        public string upLoadTime;

        public override void readFields(BmobInput input)
        {
            base.readFields(input);
            

            MapId = input.getString("mapID");
            MapName = input.getString("mapName");
            coordinate = input.getGeoPoint("position");
            //userId = input.getString("userId");
            upLoadTime = input.getString("createdAt");
            snapShot = input.getFile("snapShot");
            meta = input.getFile("meta");
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("position", coordinate);
            //output.Put("userId", userId);
            output.Put("createdAt", upLoadTime);
            output.Put("snapShot", snapShot);
            output.Put("meta", meta);
        }
    }

    public class Position
    {
        public float top;
        public float bottom;
        public float left;
        public float right;
    }
}
