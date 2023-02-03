using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
	[SerializeField] private RoomController _controller;
	[SerializeField] private Floor _floor;
	[SerializeField] private bool _debug;
	
	[Header("Rooms")]
	[SerializeField] private List<Room> _placedRooms;
	
	[Header("Doors")]
	[SerializeField] private DoorController _doorPrefab;
	[SerializeField] private GameObject _lockedDoorPrefab;
	[SerializeField] private Transform _doorParent;
	
	private const float RoomSize = 8f;
	private const float OneOverRoomSize = 1f/RoomSize;
	public const float HalfRoomSize = RoomSize*0.5f;
	
	private void Start()
	{
		foreach (Transform child in transform)
		{
			var room = child.GetComponent<Room>();
			if (room && !_placedRooms.Contains(room))
			{
				_placedRooms.Add(room);
			}
		}
		foreach (var room in _placedRooms)
		{
			room.transform.SetParent(transform);
			Vector3 pos = room.transform.localPosition * OneOverRoomSize;
			room.X = Mathf.RoundToInt(pos.x);
			room.Z = Mathf.RoundToInt(pos.z);
			room.SetGenerator(this);
			room.CheckGenerateDoors();
			_controller.AddPlacedRoom(room);
		}
	}
	
	public void PlaceRoomLocally(int x, int z, Orient connection)
	{
		Log($"Place Room Locally: {x}, {z}, connecting to {connection.ToString()}");
		if (HasRoom(x, z))
		{
			Debug.LogError("Room Exists! Do not Place Room");
			return;
		}
		var prefab = _controller.GetNextRoom(_floor);
		
		// Figure out rotation (Test door connections)
		int rot = 0;
		switch (prefab.DoorCount)
		{
			case 1:
				// Make the door face the previous X and Z values
				var doorSide = prefab.GetDoorLocalOrientations()[0];
				rot = (int)Room.GetOrientation(doorSide, connection);
				break;
			case 2:
			case 3:
				int totalConnections = 0;
				var rotations = new List<Orient>();
				
				Log($"FIND ROTATION FOR {prefab.name}");
				// Test all rotations
				for (int i = 0; i < 4; i++)
				{
					Log($"Rotation ({i})");
					int connections = 0;
					var localOrientations = prefab.GetDoorLocalOrientations();
					// Test for connections at each possible door
					foreach (var localOrientation in localOrientations)
					{
						var orientation = Room.AddOrientation(localOrientation, (Orient)i);
						(int xOffset, int zOffset) = Room.GetOffset(orientation);
						var otherRoom = GetRoom(x + xOffset, z + zOffset);
						if (otherRoom && otherRoom.HasDoorWithOrientation(Room.ReverseOrientation(orientation)))
						{
							connections += orientation == connection ? 11 : 1;
						}
						Log($" - Door Orientation {localOrientation} to {orientation} found {otherRoom}");
					}
					if (connections > totalConnections)
					{
						totalConnections = connections;
						rotations = new List<Orient>() { (Orient)i };
					}
					else if (connections == totalConnections)
					{
						rotations.Add((Orient)i);
					}
					Log($" - Connections ({connections} / {totalConnections})");
				}
				Log($"Rotation: {rot}");
				rot = (int)rotations[Random.Range(0, rotations.Count)];
				break;
			case 4:
			default:
				// Any Rotation Works
				rot = Random.Range(0, 4);
				break;
		}
		PlaceRoom(prefab, x, z, rot);
		if (NetworkManager.Instance) NetworkManager.OnCreateNewRoomLocally(prefab.Id, (int)_floor, x, z, rot);
	}

	public Room PlaceRoom(Room prefab, int x, int z, int rot)
	{
		Debug.Log($"Place room ({prefab.name}) at {x}, {z} with rotation {rot}");
		
		var room = Instantiate(prefab, transform);
		room.SetGenerator(this);
		_placedRooms.Add(room);
		_controller.AddPlacedRoom(room);
		
		room.X = x;
		room.Z = z;
		room.transform.localPosition = new Vector3(x * RoomSize, 0, z * RoomSize);
		
		room.Orientation = (Orient)rot;
		room.transform.localRotation = Quaternion.Euler(new Vector3(0, rot * 90f, 0));

		room.SetGenerator(this);
		room.CheckGenerateDoors();
		room.ShowTop(_controller.ShowRoomTops);
		
		return room;
	}
	
	private int GetDoorRotationValue(bool posZ, bool posX, bool negZ, bool negX)
	{
		return (posX ? 1 : 0) + (negZ ? 2 : 0) + (negX ? 3 : 0);
	}

	public bool HasRoom(int x, int z) => _placedRooms.Any(room => room.X == x && room.Z == z);
	public Room GetRoom(int x, int z) => _placedRooms.FirstOrDefault(room => room.X == x && room.Z == z);
	
	public DoorController CreateDoor() => Instantiate(_doorPrefab, _doorParent);
	public GameObject CreateLockedDoor() => Instantiate(_lockedDoorPrefab, _doorParent);

	private void Log(string message)
	{
		if (_debug) Debug.Log(message, gameObject);
	}
}