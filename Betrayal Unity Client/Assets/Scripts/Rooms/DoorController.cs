using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float _openSpeed;
	[SerializeField] private float _rotYClosed = 0;
	[SerializeField] private float _rotYOpen = 90;
	
	[Header("References")]
	[SerializeField] private Transform _pivot;
	[SerializeField] private GameObject _labels;
	[SerializeField] private TMP_Text _frontLabelText;
	[SerializeField] private TMP_Text _backLabelText;
	
	[Header("Debug")]
	[SerializeField, ReadOnly] private Room _room;
	[SerializeField, ReadOnly] private Orient _worldOrient;
	[SerializeField, ReadOnly] private bool _open;
	[SerializeField, ReadOnly] private bool _connected;
	[SerializeField, ReadOnly] private float _rotY;
	[SerializeField, ReadOnly] private bool _moving;
	
	public Transform Pivot => _pivot;
	public Orient WorldOrient => _worldOrient;
	public bool DoorHasConnection => _connected;
	
	public void SetRoom(Room room, Orient worldOrient)
	{
		_room = room;
		_worldOrient = worldOrient;
	}
	
	public void Open(bool createRoom = true)
	{
		if (_open) return;
		_open = true;
		_connected = true;
		_moving = true;
		if (createRoom)
		{
			_room.OnDoorOpen(_worldOrient);
			GameController.Instance.StartEventPhase(this);
		}
	}
	
	[Button]
	public void OpenCloseConnected(bool open)
	{
		if (!_connected) return;
		_open = open;
		_moving = true;
	}
	
	public void SetLabels(string forwardsRoom, string backwardsRoom)
	{
		_labels.SetActive(true);
		_frontLabelText.text = forwardsRoom;
		_backLabelText.text = backwardsRoom;
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
