using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	[SerializeField] private Transform _pivot;
	[SerializeField] private float _openSpeed;
	[SerializeField] private float _rotYClosed = 0;
	[SerializeField] private float _rotYOpen = 90;
	
	[SerializeField, ReadOnly] private Room _room;
	[SerializeField, ReadOnly] private Orient _worldOrient;
	[SerializeField, ReadOnly] private bool _open;
	[SerializeField, ReadOnly] private float _rotY;
	[SerializeField, ReadOnly] private bool _moving;
	
	public Orient WorldOrient => _worldOrient;
	
	public void SetRoom(Room room, Orient worldOrient)
	{
		_room = room;
		_worldOrient = worldOrient;
	}
	
	public void Open(bool createRoom = true)
	{
		_open = true;
		_moving = true;
		if (createRoom) _room.OnDoorOpen(_worldOrient);
	}
	
	private void Update()
	{
		if (!_moving) return;
		float goal = _open ? _rotYOpen : _rotYClosed;
		_rotY = Mathf.Lerp(_rotY, goal, _openSpeed * Time.deltaTime);
		if (Mathf.Abs(_rotY - goal) < 0.01f)
		{
			_rotY = goal;
			_moving = false;
		}
		_pivot.localRotation = Quaternion.Euler(0, _rotY, 0);
	}
}
