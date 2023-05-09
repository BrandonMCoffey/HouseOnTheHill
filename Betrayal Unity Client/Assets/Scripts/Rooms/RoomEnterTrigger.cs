using UnityEngine;

public class RoomEnterTrigger : MonoBehaviour
{
	[SerializeField] private Room _room;
	
	private void OnValidate()
	{
		if (!_room) _room = transform.GetComponent<Room>();
		if (!_room && transform.parent) _room = transform.parent.GetComponent<Room>();
		if (!_room && transform.parent.parent) _room = transform.parent.parent.GetComponent<Room>();
		
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<MovementController>())
		{
			GameController.EnterNewRoom(_room);
		}
	}
}
