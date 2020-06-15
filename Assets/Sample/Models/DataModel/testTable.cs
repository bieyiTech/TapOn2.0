using cn.bmob.io;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTable : BmobTable
{
    private string fTable;
    //以下对应云端字段名称
    public string userId { get; set; }
    public String test { get; set; }
    //public BmobBoolean cheatMode { get; set; }

    //构造函数
    public testTable(String tableName)
    {
        this.fTable = tableName;
    }

    public override string table
    {
        get
        {
            if (fTable != null)
            {
                return fTable;
            }
            return base.table;
        }
    }

    //读字段信息
    public override void readFields(BmobInput input)
    {
        base.readFields(input);

        this.userId = input.getString("userId");
        this.test = input.getString("test");
        //this.playerName = input.getString("playerName");
    }

    //写字段信息
    public override void write(BmobOutput output, bool all)
    {
        base.write(output, all);

        output.Put("userId", this.userId);
        output.Put("test", this.test);
        //output.Put("playerName", this.playerName);
    }
}
