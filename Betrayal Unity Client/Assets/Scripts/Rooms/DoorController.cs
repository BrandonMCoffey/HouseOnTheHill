using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	[SerializeField] private Collider _collider;
	[SerializeField] private GameObject _art;
	
	private RoomController _controller;
	
	public void SetController(RoomController controller) => _controller = controller;
	
	private void OnValidate()
	{
		if (!_collider) _collider = GetComponent<Collider>();
		if (!_art) _art = transform.GetChild(0).gameObject;
	}
	
	[Button]
	public void Open()
	{
		_controller.CreateRoom(transform);
		_collider.isTrigger = true;
		_art.SetActive(false);
	}
}
