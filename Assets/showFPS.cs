using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showFPS : MonoBehaviour
{
    public Text text;
    public float updateInterval = 0.5f;
    private float lastInterval;
    private int frames = 0;
    private float fps = 0;
    // Start is called before the first frame update
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow >= lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
        text.text = "fps: " + fps.ToString();
    }
}
