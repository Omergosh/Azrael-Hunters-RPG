using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    float barDisplay = 0;
    public Vector2 pos;
    public Vector2 size = new Vector2(60, 20);
    Texture2D progressBarEmpty;
    Texture2D progressBarFull;

    void Start()
    {
        //pos = transform.position;
    }
        
    void OnGUI()
    {
        //pos = transform.position;
        // draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);

        // draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), progressBarFull);
        GUI.EndGroup();

        GUI.EndGroup();

    }

    void Update()
    {
        // for this test, the bar display is linked to the current time
        // However, we'll need to set it to the player's health later
        barDisplay = Time.time * 0.05f;
    }
}
