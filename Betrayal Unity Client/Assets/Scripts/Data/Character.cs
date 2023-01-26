using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Betrayal/Character")]
public class Character : ScriptableObject
{
	[SerializeField] private string _name;
	[SerializeField] private Color _color;
	
	[Header("Traits")]
	[SerializeField] private CharacterTrait _speed;
	[SerializeField] private CharacterTrait _might;
	[SerializeField] private CharacterTrait _sanity;
	[SerializeField] private CharacterTrait _knowledge;
	
	[Header("Other Info")]
	[SerializeField] private int _age;
	[SerializeField] private string _height;
	[SerializeField] private int _weight;
	[SerializeField] private string _hobbies;
	[SerializeField] private string _birthday;
	
	public string Name => _name;
	public Color Color => _color;
	
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
	}
}
