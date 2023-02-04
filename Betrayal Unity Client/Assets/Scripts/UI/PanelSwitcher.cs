using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelSwitcher : MonoBehaviour
{
	[SerializeField] private int _defaultPanel;
	[SerializeField] private List<Canvas> _panels;
	//[SerializeField] private List<GameObject> _selectObj;
	
	private void Start()
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
		foreach (var panel in _panels)
		{
			panel.enabled = false;
		}
		_panels[num].enabled = true;
		//if (num < _selectObj.Count) SelectObj(_selectObj[num]);
	}
	
	public void SelectObj(GameObject obj)
	{
		if (obj != null && EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(obj);
		}
	}
}
