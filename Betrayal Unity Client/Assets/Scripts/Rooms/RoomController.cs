using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
	public static RoomController Instance;
	
	[Header("Generators")]
	[SerializeField] private RoomGenerator _upperFloor;
	[SerializeField] private RoomGenerator _groundFloor;
	[SerializeField] private RoomGenerator _lowerFloor;
	
	[Header("Rooms")]
	[SerializeField] private float _roomSize;
	[SerializeField] private bool _randomizeRoomOrder = true;
	[SerializeField] private bool _ignoreValidRoomFloor;
	[SerializeField] private List<Room> _rooms = new List<Room>();
	[SerializeField] private bool _showTops;

	public bool ShowRoomTops => _showTops;
	
	private Dictionary<int, Room> _activeRooms;
	
	private void Awake()
	{
		Instance = this;
		_activeRooms = new Dictionary<int, Room>();
	}
	
	private void Start()
	{
		if (_randomizeRoomOrder)
		{
			Room room;
			for (int i = 0; i < _rooms.Count; i++)
			{
				var o = Random.Range(0, _rooms.Count);
				room = _rooms[i];
				_rooms[i] = _rooms[o];
				_rooms[o] = room;
			}
		}
	}
	
	[Button(Mode = ButtonMode.InPlayMode)]
	public void ToggleShowTop() => SetShowRoomTops(!_showTops);
	public void SetShowRoomTops(bool show)
	{
		_showTops = show;
		foreach (var pair in _activeRooms)
		{
			pair.Value.ShowTop(ShowRoomTops);
		}
	}

	public void AddPlacedRoom(Room room)
	{
		_activeRooms.Add(room.Id, room);
		room.ShowTop(ShowRoomTops);
	}
	
	public void OpenAllConnectedDoors(bool open)
	{
		_upperFloor.OpenAllConnectedDoors(open);
		_groundFloor.OpenAllConnectedDoors(open);
		_lowerFloor.OpenAllConnectedDoors(open);
	}

	public static void CreateRoomRemotely(int roomId, int floor, int x, int z, int rot)
	{
		var prefab = Instance.GetRoomById(roomId);
		switch ((Floor)floor)
		{
			case Floor.Upper:
				Instance._upperFloor.PlaceRoom(prefab, x, z, rot);
				break;
			case Floor.Ground:
				Instance._groundFloor.PlaceRoom(prefab, x, z, rot);
				break;
			case Floor.Lower:
				Instance._lowerFloor.PlaceRoom(prefab, x, z, rot);
				break;
		}
	}
	
	public DoorController GetRandomOpenDoor(int floor)
	{
		switch ((Floor)floor)
		{
		case Floor.Upper:
			return Instance._upperFloor.GetRandomOpenDoor();
		case Floor.Ground:
			return Instance._groundFloor.GetRandomOpenDoor();
		case Floor.Lower:
			return Instance._lowerFloor.GetRandomOpenDoor();
		}
		return null;
	}
	
	public void MysticElevatorTeleport()
	{
		
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
			if (_ignoreValidRoomFloor || nextRoom.OnFloor(floor)) return nextRoom;
			_rooms.Add(nextRoom);
		}
		Debug.LogError("No Rooms left in stack.", gameObject);
		return null;
	}
}

public enum Floor
{
	Upper = 2,
	Ground = 1,
	Lower = 0
}

public enum Orient
{
	PosZ = 0,
	PosX = 1,
	NegZ = 2,
	NegX = 3
}