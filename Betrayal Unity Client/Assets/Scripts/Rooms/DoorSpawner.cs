using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
	[SerializeField, ReadOnly] private GameObject _door;
	
	private const float CheckDist = 1;
	
	private void Start()
	{
		var layer = LayerMask.GetMask("Door");
		bool doorExists = Physics.CheckSphere(transform.position, CheckDist, layer, QueryTriggerInteraction.Collide);
		if (!doorExists) RoomController.Instance.CreateDoor(this);
	}
	
	public void SetDoor(GameObject door) => _door = door;
}