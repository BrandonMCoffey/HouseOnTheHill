using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMainCamera : MonoBehaviour
{
	private void LateUpdate()
	{
		var t = Camera.main.transform;
		transform.LookAt(t);
		transform.Rotate(new Vector3(0, 180, 0));
	}
}
