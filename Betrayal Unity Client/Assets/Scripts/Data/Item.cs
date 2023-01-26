using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	Item,
	Weapon,
	Companion,
	Usable,
}

[CreateAssetMenu(menuName = "Betrayal/Item")]
public class Item : ScriptableObject
{
	[SerializeField] private string _name;
	[SerializeField] private bool _omen;
	[SerializeField] private ItemType _type;
	
	public string Name => _name;
	
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
	}
}
