using UnityEngine;

public class RoomColliders : MonoBehaviour
{
	[SerializeField] private GameObject _posZ;
	[SerializeField] private GameObject _posX;
	[SerializeField] private GameObject _negZ;
	[SerializeField] private GameObject _negX;
	[SerializeField] private GameObject _posZDoor;
	[SerializeField] private GameObject _posXDoor;
	[SerializeField] private GameObject _negZDoor;
	[SerializeField] private GameObject _negXDoor;
	
	public void SetColliders(bool posZDoor, bool posXDoor, bool negZDoor, bool negXDoor)
	{
		_posZ.SetActive(!posZDoor);
		_posX.SetActive(!posXDoor);
		_negZ.SetActive(!negZDoor);
		_negX.SetActive(!negXDoor);
		_posZDoor.SetActive(posZDoor);
		_posXDoor.SetActive(posXDoor);
		_negZDoor.SetActive(negZDoor);
		_negXDoor.SetActive(negXDoor);
	}
}
