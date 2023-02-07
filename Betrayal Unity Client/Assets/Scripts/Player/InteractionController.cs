using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
	[SerializeField] private GameObject _camera;
	[SerializeField] private LayerMask _interactMask;
	[SerializeField] private float _interactDistance = 2;
	
	public void Interact()
	{
		if (Physics.Raycast(transform.position, transform.forward, out var hit, _interactDistance, _interactMask))
		{
			var door = hit.collider.transform.parent.GetComponent<DoorController>();
			if (door)
			{
				door.Open();
			}
		}
	}

	public void SetCameraActive(bool active) => _camera.SetActive(active);
}
