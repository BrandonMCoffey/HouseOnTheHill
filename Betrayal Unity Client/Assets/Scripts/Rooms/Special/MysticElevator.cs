using CoffeyUtils;
using UnityEngine;

public class MysticElevator : MonoBehaviour
{
	[SerializeField] private Room _room;
	
	[SerializeField, ReadOnly] private bool _elevatorUsed;
	[SerializeField, ReadOnly] private bool _inElevator;
	[SerializeField, ReadOnly] private bool _canUseElevator;
	
	private void OnEnable()
	{
		PlayerInputManager.PrimaryInteract += AttemptUseElevator;
	}
	
	private void OnDisable()
	{
		PlayerInputManager.PrimaryInteract -= AttemptUseElevator;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<MovementController>())
		{
			_inElevator = true;
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<MovementController>())
		{
			_inElevator = false;
		}
	}
	
	private void Update()
	{
		if (_elevatorUsed || !GameController.CurrentTurn)
		{
			_canUseElevator = false;
			return;
		}
		if (_inElevator != _canUseElevator)
		{
			_canUseElevator = _inElevator;
			CanvasController.OpenExplorationInteractPopup("Press E to use Elevator", _inElevator);
		}
	}
	
	private void AttemptUseElevator()
	{
		if (!_canUseElevator) return;
		
		// TODO: Dice Roll 0-4 and Damage on 0
		MoveElevator(Random.Range(0, 3));
	}
	
	[Button]
	private void MoveElevator(int floor)
	{
		var door = RoomController.Instance.GetRandomOpenDoor(floor);
		if (!door) return;
		Debug.Log("Move To " + door.gameObject.name, door.gameObject);
		_elevatorUsed = true;
	}
}
