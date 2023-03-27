using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableIfCurrentTurn : MonoBehaviour
{
	[SerializeField] private Button _button;
	
	private void OnValidate()
	{
		if (!_button) _button = GetComponent<Button>();
	}
	
	private void OnEnable()
	{
		GameController.OnUpdatePhase += CheckPhase;
	}
	
	private void OnDisable()
	{
		GameController.OnUpdatePhase -= CheckPhase;
	}
	
	private void CheckPhase()
	{
		_button.interactable = GameController.CurrentTurn;
	}
}
