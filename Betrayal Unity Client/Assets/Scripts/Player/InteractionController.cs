using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
	[SerializeField] private GameObject _camera;
	[SerializeField] private LayerMask _interactMask;
	[SerializeField] private float _interactDistance = 2;
	[SerializeField, ReadOnly] private bool _canOpenDoor;
	
	public void Interact()
	{
		if (Physics.Raycast(transform.position, transform.forward, out var hit, _interactDistance, _interactMask))
		{
			if (_canOpenDoor)
			{
				var door = hit.collider.transform.parent.GetComponent<DoorController>();
				if (door)
				{
					door.Open();
				}
			}
			var collectable = hit.collider.GetComponent<CollectableItem>();
			if (collectable)
			{
				collectable.CollectItem();
			}
		}
	}

	public void SetCameraActive(bool active) => _camera.SetActive(active);
	public void SetCanOpenDoor(bool canOpenDoor) => _canOpenDoor = canOpenDoor;
}
