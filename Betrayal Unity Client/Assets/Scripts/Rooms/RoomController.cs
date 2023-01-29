using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
	public static RoomController Instance;
	
	[Header("Rooms")]
	[SerializeField] private float _roomSize;
	[SerializeField] private List<Room> _rooms;
	
	[Header("Doors")]
	[SerializeField] private DoorController _doorPrefab;
	[SerializeField] private Transform _doorParent;
	[SerializeField, ReadOnly] private List<DoorController> _doors;
	
	private void Awake()
	{
		Instance = this;
	}
	
	public void CreateDoor(DoorSpawner spawner)
	{
		var door = Instantiate(_doorPrefab, _doorParent);
		door.transform.SetPositionAndRotation(spawner.transform.position, spawner.transform.rotation);
		door.SetController(this);
		Destroy(spawner.gameObject);
		_doors.Add(door);
	}
	
	public void CreateRoom(Transform door)
	{
		var room = Instantiate(GetRandomRoom(), transform);
		
		var roomPos = door.position + door.forward * _roomSize * 0.5f;
		room.transform.position = roomPos;
	}
	
	private Room GetRandomRoom()
	{
		int index = Random.Range(0, _rooms.Count);
		var room = _rooms[index];
		_rooms.RemoveAt(index);
		return room;
	}
}
