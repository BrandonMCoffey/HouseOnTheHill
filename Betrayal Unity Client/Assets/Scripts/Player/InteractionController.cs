using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
	[SerializeField] private KeyCode _interactKey = KeyCode.E;
	[SerializeField] private LayerMask _interactMask;
	[SerializeField] private float _interactDistance = 2;
	
	private void Update()
	{
		if (Input.GetKeyDown(_interactKey))
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
	}
}
