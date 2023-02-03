using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
	public static RoomController Instance;
	
	[Header("Generators")]
	[SerializeField] private RoomGenerator _upperFloor;
	[SerializeField] private RoomGenerator _groundFloor;
	[SerializeField] private RoomGenerator _lowerFloor;
	
	[Header("Rooms")]
	[SerializeField] private float _roomSize;
	[SerializeField] private List<Room> _rooms = new List<Room>();
	
	[Header("Doors")]
	[SerializeField] private DoorController _doorPrefab;
	[SerializeField] private Transform _doorParent;
	[SerializeField, ReadOnly] private List<DoorController> _doors;
	
	private Dictionary<int, Room> _activeRooms;
	
	private void Awake()
	{
		Instance = this;
		_activeRooms = new Dictionary<int, Room>();
	}
	
	private void Start()
	{
		foreach (Transform child in transform)
		{
			var room = child.GetComponent<Room>();
			if (room) _activeRooms.Add(room.Id, room);
		}
	}
	
	public void CreateDoor(DoorSpawner spawner)
	{
		var door = Instantiate(_doorPrefab, _doorParent);
		door.transform.SetPositionAndRotation(spawner.transform.position, spawner.transform.rotation);
		//door.SetController(this);
		Destroy(spawner.gameObject);
		_doors.Add(door);
	}
	
	public void CreateRoomLocally(Transform door)
	{
		var room = Instantiate(GetRandomRoom(), transform);
		
		var roomPos = door.position + door.forward * _roomSize * 0.5f;
		room.transform.position = roomPos;
		
		_activeRooms.Add(room.Id, room);
		
		NetworkManager.OnCreateNewRoomLocally(room.Id, roomPos, room.transform.eulerAngles);
	}
	
	public static void CreateRoomRemotely(int roomId, Vector3 pos, Vector3 rot)
	{
		var room = Instantiate(Instance.GetRoomById(roomId), Instance.transform);
		
		room.transform.position = pos;
		room.transform.rotation = Quaternion.Euler(rot);
		
		Instance._activeRooms.Add(room.Id, room);
	}
	
	private Room GetRoomById(int roomId)
	{
		foreach (var room in _rooms)
		{
			if (room.Id == roomId)
			{
				_rooms.Remove(room);
				return room;
			}
		}
		Debug.LogError("Unable to find room with id " + roomId, gameObject);
		return null;
	}
	
	public Room GetNextRoom(Floor floor)
	{
		for (int i = 0; i < _rooms.Count; i++)
		{
			var nextRoom = _rooms[0];
			_rooms.RemoveAt(0);
			if (nextRoom.OnFloor(floor)) return nextRoom;
			_rooms.Add(nextRoom);
		}
		Debug.LogError("No Rooms left in stack.", gameObject);
		return null;
	}
	
	private Room GetRandomRoom()
	{
		int index = Random.Range(0, _rooms.Count);
		var room = _rooms[index];
		_rooms.RemoveAt(index);
		return room;
	}
}

public enum Floor
{
	Upper,
	Ground,
	Lower
}

public enum Orient
{
	PosZ = 0,
	PosX = 1,
	NegZ = 2,
	NegX = 3
}