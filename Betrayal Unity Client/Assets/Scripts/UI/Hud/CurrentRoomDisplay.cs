using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentRoomDisplay : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;
	
	private void OnValidate()
	{
		if (!_text) _text = GetComponent<TMP_Text>();
	}
	
	private void Awake()
	{
		GameController.UpdateCurrentRoom += UpdateRoomName;
	}
	
	private void UpdateRoomName(string name)
	{
		_text.text = name;
	}
}
