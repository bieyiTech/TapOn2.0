using cn.bmob.io;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapOn.Models
{
    public class Marks : BmobTable
    {
        public static string table_name = "Mark";

        public BmobGeoPoint coordinate;
        public BmobInt type;
        public string userId;

        public BmobFile snapShot;
        public string upLoadTime;

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            coordinate = input.getGeoPoint("position");
            type = input.getInt("type");
            userId = input.getString("userId");
            upLoadTime = input.getString("createdAt");
            snapShot = input.getFile("snapShot");
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("position", coordinate);
            output.Put("type", type);
            output.Put("userId", userId);
            output.Put("createdAt", upLoadTime);
            output.Put("snapShot", snapShot);
        }
    }
}
