using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;
using TMPro;

public class ObjectivesList : MonoBehaviour
{
	[SerializeField] private List<TMP_Text> _objectiveTexts = new List<TMP_Text>();
	
	private void OnEnable()
	{
		GameController.OnUpdateObjectives += UpdateObjectives;
	}
	
	private void OnDisable()
	{
		GameController.OnUpdateObjectives -= UpdateObjectives;
	}
	
	[Button(Mode = RuntimeMode.OnlyPlaying)]
	private void UpdateObjectives()
	{
		var objectives = GameController.Instance.GetObjectives();
		int i = 0;
		foreach (var objective in objectives)
		{
			_objectiveTexts[i++].text = objective;
		}
		for (; i < _objectiveTexts.Count; i++)
		{
			_objectiveTexts[i].text = "";
		}
	}
}
