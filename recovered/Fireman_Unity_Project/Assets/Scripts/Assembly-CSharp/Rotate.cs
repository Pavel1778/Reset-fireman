using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float speed = 0.1f;

	public float angle;

	public void Update()
	{
		base.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
	}

	public void FixedUpdate()
	{
		angle += speed * Time.deltaTime;
	}
}
