using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;
using TapOn.Api;
using TapOn.Constants;
using System.IO;

public class MarkManager : MonoBehaviour
{
    public float Saturation = 0.8f;
    public float Brightness = 0.8f;
    public SnapShotLoad snapShotLoad;

    public GameObject AddMark(Mark mark)
    {
        Vector3 pos = Globals.instance.mapController.ConvertCoordinateToWorld(new TencentMap.CoordinateSystem.Coordinate(mark.coordinate.Latitude.Get(), mark.coordinate.Longitude.Get()));

        GameObject tempMark = (GameObject)Instantiate(Globals.instance.marker, GetComponent<MarkManager>().transform, true);
        tempMark.transform.localScale *= 3.2f;
        tempMark.transform.position = pos + 0.5f * new Vector3(0, 0.01f, tempMark.transform.localScale.y * tempMark.GetComponent<SpriteRenderer>().bounds.size.y);
        tempMark.GetComponent<Renderer>().sortingOrder = 1;
        tempMark.GetComponent<LocationInfo>().mark = mark;
        tempMark.layer = 256;

        //// 计算颜色 HSV->RGB
        string[] temp = mark.updatedAt.Split(' ');
        string[] temp2 = temp[1].Split(':');
        float Hue = (int.Parse(temp2[0]) * 15 + int.Parse(temp2[1]) / 4) * 1.0f / 360;
        tempMark.GetComponent<Renderer>().material.color = Color.HSVToRGB(Hue, Saturation, Brightness);

        StartCoroutine(snapShotLoad.LoadSnapShot(mark.snapShot.url, mark.snapShot.filename, "picture", tempMark));

        return tempMark;
    }
    
}
