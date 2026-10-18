using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class FixedCamera : MonoBehaviour
{
	public int FixedWidth = 1024;

	public int FixedHeight = 600;

	private Camera _camera;

	private float _ratioX = 1f;

	private float _ratioY = 1f;

	public void OnEnable()
	{
		_camera = GetComponent<Camera>();
		_camera.aspect = (float)FixedWidth / (float)FixedHeight;
		_camera.orthographic = true;
		_camera.orthographicSize = FixedHeight >> 1;
		_camera.farClipPlane = 1000f;
		_camera.nearClipPlane = -1000f;
		_ratioX = (float)FixedWidth / (float)Screen.width;
		_ratioY = (float)FixedHeight / (float)Screen.height;
	}

	public void OnDrawGizmos()
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.color = Color.green;
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		if (_camera.orthographic)
		{
			float z = _camera.farClipPlane - _camera.nearClipPlane;
			float z2 = (_camera.farClipPlane + _camera.nearClipPlane) * 0.5f;
			Gizmos.DrawWireCube(new Vector3(0f, 0f, z2), new Vector3((_camera.orthographicSize + _camera.orthographicSize) * _camera.aspect, _camera.orthographicSize + _camera.orthographicSize, z));
		}
		else
		{
			Gizmos.DrawFrustum(Vector3.zero, _camera.fieldOfView, _camera.farClipPlane, _camera.nearClipPlane, _camera.aspect);
		}
		Gizmos.matrix = matrix;
	}

	public float Screen2BufferX(float v)
	{
		return v * _ratioX;
	}

	public float Screen2BufferY(float v)
	{
		return (float)FixedHeight - v * _ratioY;
	}
}
