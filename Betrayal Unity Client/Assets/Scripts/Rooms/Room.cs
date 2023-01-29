using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] private string _name;
	
	[Header("References")]
	[SerializeField] private List<DoorSpawner> _doors;
	[SerializeField] private List<Light> _lights;
	[SerializeField] private GameObject _topToRemove;
	
	[Header("Debug")]
	[SerializeField, ReadOnly] private bool _showTop = true;
	
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
	}
	
	[Button]
	private void RefreshReferences()
	{
		_doors = gameObject.GetComponentsInChildren<DoorSpawner>().ToList();
		_lights = gameObject.GetComponentsInChildren<Light>().ToList();
		var top = RecursiveFindChild(transform, "Top").gameObject;
		if (top) _topToRemove = top;
	}
	
	[Button]
	private void ToggleShowTop() => ShowTop(!_showTop);
	private void ShowTop(bool show)
	{
		_showTop = show;
		_topToRemove.SetActive(show);
	}
	
	Transform RecursiveFindChild(Transform parent, string childName)
	{
		childName = childName.ToLower();
		foreach (Transform child in parent)
		{
			if (child.name.ToLower().Equals(childName))
			{
				return child;
			}
			else
			{
				Transform found = RecursiveFindChild(child, childName);
				if (found != null)
				{
					return found;
				}
			}
		}
		return null;
	}
}
