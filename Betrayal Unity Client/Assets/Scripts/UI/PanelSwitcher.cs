using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelSwitcher : MonoBehaviour
{
	[SerializeField] private int _defaultPanel;
	[SerializeField, ReadOnly] private int _currentPanel;
	[SerializeField] private List<CanvasGroup> _panels;
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
			SetPanelActive(panel, false);
		}
		SetPanelActive(_panels[num]);
		//if (num < _selectObj.Count) SelectObj(_selectObj[num]);
	}
	
	public void SelectObj(GameObject obj)
	{
		if (obj != null && EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(obj);
		}
	}
	
	private void SetPanelActive(CanvasGroup group, bool active = true)
	{
		group.alpha = active ? 1 : 0;
		group.blocksRaycasts = active;
		group.interactable = active;
	}
}
