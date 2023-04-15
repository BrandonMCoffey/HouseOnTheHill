using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDecoration : MonoBehaviour
{
	[SerializeField] private Light _roomLight;
	[SerializeField] private float _basicLightIntensity = 3;
	[SerializeField] private Color _basicLightColor = Color.white;
	[SerializeField] private float _omenLightIntensity = 2;
	[SerializeField] private Color _omenLightColor = Color.green;
	[SerializeField] private float _range = 10;
	
	public void UpdateLights(Room room)
	{
		_roomLight.intensity = room.Omen ? _omenLightIntensity : _basicLightIntensity;
		_roomLight.color = room.Omen ? _omenLightColor : _basicLightColor;
		_roomLight.range = _range;
	}
}
