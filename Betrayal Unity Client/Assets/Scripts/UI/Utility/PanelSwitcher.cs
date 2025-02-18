﻿using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelSwitcher : MonoBehaviour
{
	[SerializeField] private int _defaultPanel;
	[SerializeField] private List<Canvas> _panels;
	[SerializeField, ReadOnly] private int _openPanel;
	//[SerializeField] private List<GameObject> _selectObj;
	
	[SerializeField] private bool _debug;

	public int CurrentlyOpenPanel => _openPanel;
	
	private void Awake()
	{
		OpenPanel(_defaultPanel);
	}
	
	[Button]
	public void OpenPanel(int num)
	{
		if (num < 0 || num >= _panels.Count) 
		{
			Debug.LogError("Cannot open panel #" + num, gameObject);
			return;
		}
		_openPanel = num;
		foreach (var panel in _panels)
		{
			panel.enabled = false;
		}
		_panels[num].enabled = true;
		//if (num < _selectObj.Count) SelectObj(_selectObj[num]);
		
		if (_debug) Debug.Log($"Open Panel {num} ({_panels[num].name})");
	}
	
	public void SelectObj(GameObject obj)
	{
		if (obj != null && EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(obj);
		}
	}
}
