using System.Collections;
using UnityEngine;

/// <summary>
/// Snaps the attached GameObject to a Pixel Perfect X and Y position.
/// </summary>
[ExecuteInEditMode]
public class PixelSnapScript : MonoBehaviour
{
    public int PixelsPerUnit = 16;

    private float _lastX;
    private float _lastY;

    // Snap the attached GameObject to it's Pixel Perfect position.
    public void Update()
    {
        if (gameObject.transform.position.x == _lastX && gameObject.transform.position.y == _lastY) return;

        gameObject.transform.position = new Vector3(((float)((int)(gameObject.transform.position.x * PixelsPerUnit)) / PixelsPerUnit),
                                                    ((float)((int)(gameObject.transform.position.y * PixelsPerUnit)) / PixelsPerUnit),
                                                    gameObject.transform.position.z);
        _lastX = gameObject.transform.position.x;
        _lastY = gameObject.transform.position.y;
    }

    // Move the attached GameObject by a certain number of Pixels.
    public void Move(int pixelX, int pixelY)
    {
        gameObject.transform.position = new Vector3(((float)((int)(gameObject.transform.position.x * PixelsPerUnit) + pixelX) / PixelsPerUnit),
                                                    ((float)((int)(gameObject.transform.position.y * PixelsPerUnit) + pixelY) / PixelsPerUnit),
                                                    gameObject.transform.position.z);
        _lastX = gameObject.transform.position.x;
        _lastY = gameObject.transform.position.y;
    }
}