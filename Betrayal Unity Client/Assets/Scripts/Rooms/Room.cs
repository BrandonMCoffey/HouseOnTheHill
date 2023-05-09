using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
	[SerializeField] private int _id;
	[SerializeField] private string _name;
	
	[Header("Floors")]
	[SerializeField] private bool _upperFloor;
	[SerializeField] private bool _groundFloor;
	[SerializeField] private bool _lowerFloor;
	
	[Header("Event")]
	[SerializeField] private bool _event;
	[SerializeField] private bool _omen;
	[SerializeField, ShowIf("_omen")] private Transform _omenLocation;
	[SerializeField] private bool _item;
	[SerializeField, ShowIf("_item")] private Transform _itemLocation;
	[SerializeField] private bool _secondItem;
	[SerializeField, ShowIf("_secondItem")] private Transform _secondItemLocation;
	
	[Header("Doors")]
	[SerializeField] private bool _localPosZDoor;
	[SerializeField] private bool _localPosXDoor;
	[SerializeField] private bool _localNegZDoor;
	[SerializeField] private bool _localNegXDoor;
	[SerializeField, ReadOnly] private int _doorCount;
	
	[Header("References")]
	[SerializeField] private RoomColliders _colliders;
	[SerializeField] private GameObject _topToRemove;
	[SerializeField] private RoomDecoration _decoration;
	
	[Header("Debug")]
	[SerializeField, ReadOnly] private RoomGenerator _generator;
	[SerializeField, ReadOnly] private bool _showTop = true;
	[SerializeField, ReadOnly] private int _z;
	[SerializeField, ReadOnly] private int _x;
	[SerializeField, ReadOnly] private Orient _orientation = Orient.PosZ;
	[SerializeField, ReadOnly] private List<DoorController> _doors = new List<DoorController>();

	public int Id => _id;
	public bool Omen => _omen;
	public void SetId(int id) => _id = id;
	public string Name => _name;
	public int Z { get => _z; set => _z = value; }
	public int X { get => _x; set => _x = value; }
	public Orient Orientation { get => _orientation; set => _orientation = value; }
	public int DoorCount => _doorCount;

	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
		if (!_decoration) _decoration = GetComponentInChildren<RoomDecoration>();
		_doorCount = (_localPosZDoor ? 1 : 0) + (_localPosXDoor ? 1 : 0) + (_localNegZDoor ? 1 : 0) + (_localNegXDoor ? 1 : 0);
	}

	public void SetGenerator(RoomGenerator generator) => _generator = generator;
	
	[Button(Spacing = 10)]
	private void RefreshReferences()
	{
		_colliders = gameObject.GetComponentInChildren<RoomColliders>();
		var top = RecursiveFindChild(transform, "top").gameObject;
		if (top) _topToRemove = top;
	}
	
	[Button]
	private void RefreshColliders()
	{
		_colliders.SetColliders(_localPosZDoor, _localPosXDoor, _localNegZDoor, _localNegXDoor);
	}
	
	[Button]
	private void UpdateDecorations()
	{
		_decoration.UpdateLights(this);
	}
	
	[Button]
	private void ResetMeshColliders()
	{
		var list = new List<MeshRenderer>();
		var colliders = GetRecursiveAllOfType<MeshRenderer>(list, transform);
		foreach (var c in colliders)
		{
			c.enabled = true;
		}
	}
	private List<T> GetRecursiveAllOfType<T>(List<T> list, Transform parent)
	{
		var all = transform.GetComponentsInChildren<T>();
		foreach (var t in all)
		{
			if (!list.Contains(t)) list.Add(t);
		}
		foreach (Transform child in parent)
		{
			list = GetRecursiveAllOfType(list, child);
		}
		return list;
	}
	
	private static Transform RecursiveFindChild(Transform parent, string childName)
	{
		foreach (Transform child in parent)
		{
			if (child.name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)) return child;
			Transform found = RecursiveFindChild(child, childName);
			if (found != null) return found;
		}
		return null;
	}
	
	[Button(Spacing = 10)]
	private void ToggleShowTop() => ShowTop(!_showTop);
	public void ShowTop(bool show)
	{
		_showTop = show;
		_topToRemove.SetActive(show);
	}
	
	public void OnDoorOpen(Orient dir)
	{
		(int x, int z) = GetOffset(dir);
		_generator.PlaceRoomLocally(X + x, Z + z, ReverseOrientation(dir));
	}
	
	public void RunEvent(bool local)
	{
		if (!local) return;
		if (_event) EventController.Instance.CreateEvent(this);
		if (_omen) EventController.Instance.CreateOmen(this);
		if (_item && _secondItem) EventController.Instance.CreateIem(this);
		else if (_item || _secondItem) EventController.Instance.CreateTwoItems(this);
		if (!_event && !_omen && !_item && !_secondItem) StartCoroutine(DelayCheckEndTurn());
	}

	private static IEnumerator DelayCheckEndTurn()
	{
		yield return new WaitForSeconds(Random.Range(5f, 6f));
		GameController.Instance.CheckCanEndTurn();
	}
	
	public DoorController GetRandomOpenDoor() => _doors.FirstOrDefault(d => !d.DoorHasConnection);
	
	[Button(Mode = ButtonMode.InPlayMode)]
	public void CheckGenerateDoors()
	{
		GenerateDoor(AddOrientation(_orientation, Orient.PosZ), _localPosZDoor);
		GenerateDoor(AddOrientation(_orientation, Orient.PosX), _localPosXDoor);
		GenerateDoor(AddOrientation(_orientation, Orient.NegZ), _localNegZDoor);
		GenerateDoor(AddOrientation(_orientation, Orient.NegX), _localNegXDoor);

		void GenerateDoor(Orient worldOrient, bool create)
		{
			(int x, int z) = GetOffset(worldOrient);
			var connectedRoom = _generator.GetRoom(X + x, Z + z);
			if (!connectedRoom && create)
			{
				// Create a door that can be explored (No connection until the player opens it)
				var door = _generator.CreateDoor();
				door.SetRoom(this, worldOrient);
				SetDoorTransform(door.transform, worldOrient);
				_doors.Add(door);
			}
			else if (connectedRoom)
			{
				// Connected room already exists
				var door = connectedRoom.GetDoor(ReverseOrientation(worldOrient));
				switch (door != null, create)
				{
					case (true, true):
						// Existing door with a connection
						door.Open(false);
						door.SetLabels(connectedRoom.Name, Name);
						break;
					case (false, true):
						// Create a locked door -- no connection
						SetDoorTransform(_generator.CreateLockedDoor().transform, worldOrient, true);
						break;
					case (true, false):
						// Existing door -- but no connection -- lock other door
						connectedRoom.DestroyDoor(door);
						SetDoorTransform(_generator.CreateLockedDoor().transform, worldOrient);
						break;
				}
			}
		}
	}
	
	public bool OnFloor(Floor floor)
	{
		return floor switch
		{
			Floor.Upper => _upperFloor,
			Floor.Ground => _groundFloor,
			Floor.Lower => _lowerFloor,
			_ => false
		};
	}

	public static Orient GetOrientation(Orient x, Orient y) => (Orient)(((int)y - (int)x + 4) % 4);
	public static Orient AddOrientation(Orient x, Orient y) => (Orient)(((int)x + (int)y) % 4);
	//private Orient GetOrientation(Orient orient) => GetOrientation(orient, _orientation);

	public static Orient ReverseOrientation(Orient orient)
	{
		return orient switch
		{
			Orient.PosZ => Orient.NegZ,
			Orient.PosX => Orient.NegX,
			Orient.NegZ => Orient.PosZ,
			Orient.NegX => Orient.PosX,
			_ => Orient.PosZ
		};
	}

	public static (int, int) GetOffset(Orient orient)
	{
		return orient switch
		{
			Orient.PosZ => (0, 1),
			Orient.PosX => (1, 0),
			Orient.NegZ => (0, -1),
			Orient.NegX => (-1, 0),
			_ => (0, 0)
		};
	}

	public bool HasDoorWithOrientation(Orient orient) => _doors.Any(door => door.WorldOrient == orient);
	private DoorController GetDoor(Orient orient) => _doors.FirstOrDefault(door => door.WorldOrient == orient);

	public List<Orient> GetDoorLocalOrientations()
	{
		var list = new List<Orient>();
		if (_localPosZDoor) list.Add(Orient.PosZ);
		if (_localPosXDoor) list.Add(Orient.PosX);
		if (_localNegZDoor) list.Add(Orient.NegZ);
		if (_localNegXDoor) list.Add(Orient.NegX);
		return list;
	}
	public List<Orient> GetDoorWorldOrientations() => _doors.Select(door => door.WorldOrient).ToList();

	// Side (0 = PosZ, 1 = PosX, 2 = NegZ, 3 = NegX)
	private void SetDoorTransform(Transform door, Orient orient, bool reverse = false)
	{
		var offset = Vector3.zero;
		if (orient == Orient.PosZ) offset.z += RoomGenerator.HalfRoomSize;
		else if (orient == Orient.PosX) offset.x += RoomGenerator.HalfRoomSize;
		else if (orient == Orient.NegZ) offset.z -= RoomGenerator.HalfRoomSize;
		else if (orient == Orient.NegX) offset.x -= RoomGenerator.HalfRoomSize;
		var rot = Quaternion.Euler(new Vector3(0, (int)orient * 90f + (reverse ? 180f : 0f), 0));
		door.SetPositionAndRotation(transform.position + offset, rot);
	}

	private void DestroyDoor(DoorController door)
	{
		if (_doors.Contains(door))
		{
			_doors.Remove(door);
			Destroy(door.gameObject);
		}
	}
}
