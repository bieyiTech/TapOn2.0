using cn.bmob.io;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TapOn.Models.DataModels
{
    /*public class BmobMark : BmobTable
    {
        public static string table_name = "Mark";

        public string MapId;
        public string MapName;

        public List<BmobProp> props;

        public BmobGeoPoint coordinate;
        //public string userId;

        public BmobFile snapShot;
        public string upLoadTime;

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            props = new List<BmobProp>();

            MapId = input.getString("mapID");
            MapName = input.getString("mapName");
            coordinate = input.getGeoPoint("position");
            //userId = input.getString("userId");
            upLoadTime = input.getString("createdAt");
            snapShot = input.getFile("snapShot");
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("position", coordinate);
            //output.Put("userId", userId);
            output.Put("createdAt", upLoadTime);
            output.Put("snapShot", snapShot);
        }
    }*/

    public class BmobModel : BmobTable
    {
        public static string table_name = "Model";

        public string modelName;
        public BmobFile preview;
        public BmobFile model;
        public BmobInt modelType;

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            modelType = input.getInt("modelType");
            modelName = input.getString("modelName");
            preview = input.getFile("preview");
            model = input.getFile("model");
        }
    }

    public class Prop : BmobTable
    {
        public static string table_name = "Prop";

        public BmobDouble pos_x;
        public BmobDouble pos_y;
        public BmobDouble pos_z;

        public BmobDouble rot_w;
        public BmobDouble rot_x;
        public BmobDouble rot_y;
        public BmobDouble rot_z;

        public BmobDouble scale_x;
        public BmobDouble scale_y;
        public BmobDouble scale_z;

        public GameObject instance;

        public BmobInt type;
        public string text;
        public BmobFile texture;
        public byte[] texture_byte;
        public BmobFile video;
        public BmobPointer<BmobModel> model;

        public BmobPointer<Mark> mark;

        public BmobInt version;

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            pos_x = input.getDouble("pos_x");
            pos_y = input.getDouble("pos_y");
            pos_z = input.getDouble("pos_z");
            rot_w = input.getDouble("rot_w");
            rot_x = input.getDouble("rot_x");
            rot_y = input.getDouble("rot_y");
            rot_z = input.getDouble("rot_z");
            scale_x = input.getDouble("scale_x");
            scale_y = input.getDouble("scale_y");
            scale_z = input.getDouble("scale_z");

            type = input.getInt("type");
            text = input.getString("text");
            texture = input.getFile("texture");
            video = input.getFile("video");

            model = input.Get<BmobPointer<BmobModel>>("model");
            mark = input.Get<BmobPointer<Mark>>("mark");

            version = input.getInt("version");
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("pos_x", pos_x);
            output.Put("pos_y", pos_y);
            output.Put("pos_z", pos_z);
            output.Put("rot_w", rot_w);
            output.Put("rot_x", rot_x);
            output.Put("rot_y", rot_y);
            output.Put("rot_z", rot_z);
            output.Put("scale_x", scale_x);
            output.Put("scale_y", scale_y);
            output.Put("scale_z", scale_z);
            output.Put("type", type);
            if (type.Get() == 0) output.Put("text", text);
            if (type.Get() == 1) output.Put("texture", texture);
            if (type.Get() == 2) output.Put("video", video);
            if (type.Get() == 3) output.Put("model", model);

            output.Put("mark", mark);
            output.Put("vesion", version);

        }
    }
}
