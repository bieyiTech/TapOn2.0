using cn.bmob.api;
using cn.bmob.tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BmobTest : MonoBehaviour
{
    private static BmobUnity Bmob;
    //对应要操作的数据表
    public const string TABLE_NAME = "Mark";
    //接下来要操作的数据的数据
    private testTable ttt = new testTable(TABLE_NAME);

    private void createData()
    {
        //设置值    
        //System.Random rnd = new System.Random();
        ttt.userId = "22";
        ttt.test = "dolly";
        //gameObject.cheatMode = false;

        //保存数据
        var future = Bmob.CreateTaskAsync(ttt);
        //异步显示返回的数据
        //FinishedCallback(future.Result, resultText);
    }
    // Start is called before the first frame update
    void Start()
    {
        BmobDebug.Register(print);
        BmobDebug.level = BmobDebug.Level.TRACE;
        Bmob = gameObject.GetComponent<BmobUnity>();
        createData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
