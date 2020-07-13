using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapOn.Models.DataModels;
using TapOn.Api;

public class MarkManager : MonoBehaviour
{
    public float Saturation = 0.6f;
    public float Brightness = 0.8f;
    public List<Mark> marks;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void AddMark(Mark mark)
    {
        Vector3 pos = MapApi.map.ConvertCoordinateToWorld(mark.coordinate);

        //GameObject tempMark = (GameObject)Instantiate(MapApi.prefabs.marker, GetComponent<MarkManager>().transform, true);
        //tempMark.transform.position = pos + 0.5f * new Vector3(0, 0, tempMark.transform.localScale.y * tempMark.GetComponent<SpriteRenderer>().bounds.size.y);

        //// 计算颜色 HSV->RGB
        //string[] temp = mark.date.Split(' ');
        //string[] temp2 = temp[1].Split(':');
        //float Hue = (int.Parse(temp2[0]) * 15 + int.Parse(temp2[1]) / 4) * 1.0f / 360;
        //tempMark.GetComponent<Renderer>().material.color = Color.HSVToRGB(Hue, Saturation, Brightness);

    }
    
}
