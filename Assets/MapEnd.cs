using System.Collections;
using System.Collections.Generic;
using TapOn.Api;
using UnityEngine;

public class MapEnd : MonoBehaviour
{
    public float velocity_x;
    public float velocity_y;
    public float a = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity_x -= a * Time.deltaTime;
        velocity_y -= a * Time.deltaTime;
        if (velocity_x < 0) velocity_x = 0;
        if (velocity_y < 0) velocity_y = 0;
        if (velocity_x == 0 && velocity_y == 0) { }
        else
        {
            Debug.LogError(velocity_y);
            MapApi.MoveMap(-velocity_x * Time.deltaTime, velocity_y * Time.deltaTime);
        }
        }
}
