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
	[Header("Data")]
	[SerializeField] private int _id;
	[SerializeField] private string _name;
	[SerializeField] private bool _omen;
	[SerializeField] private ItemType _type;

	[Header("Other")]
	[SerializeField] private Sprite _icon;
	[SerializeField, TextArea] private string _description;
	
	public int Id => _id;
	public void SetId(int id) => _id = id;
	public string Name => _name;
	public bool Omen => _omen;
	public ItemType Type => _type;
	public string Description => _description;
	public Sprite IconSprite => _icon;
	
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
	}
}
