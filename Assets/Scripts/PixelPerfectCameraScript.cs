using UnityEngine;

/// <summary>
/// Adjusts the Camera's Orthographic Size to create a Pixel Perfect Camera at any resolution.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelPerfectCameraScript : MonoBehaviour
{
	public Camera Camera
	{
		get { return _camera ?? (_camera = GetComponent<Camera>()); }
	}

	private Camera _camera;

	public int PixelsPerUnit = 16;

	private int _lastHeight;
	private int _lastPPU;

	public void Update()
	{
		if (_lastHeight == Screen.height && _lastPPU == PixelsPerUnit) return;

		_lastHeight = Screen.height;
		_lastPPU = PixelsPerUnit;

		Camera.orthographicSize = Screen.height / (PixelsPerUnit * 2f);
	}
}