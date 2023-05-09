using UnityEngine;

public enum Trait
{
	Speed,
	Might,
	Sanity,
	Knowledge
}

[CreateAssetMenu(menuName = "Betrayal/Character")]
public class Character : ScriptableObject
{
	[SerializeField] private string _name;
	[SerializeField] private Color _color;
	[SerializeField] private GameObject _art;
	
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
	public GameObject Art => _art;
	
	public int GetTraitIndex(Trait trait) => GetTrait(trait).Index;
	public int GetTraitValue(Trait trait, int index) => GetTrait(trait).GetValue(index);
	
	public int GetDefaultTraitValue(Trait trait)
	{
		var t = GetTrait(trait);
		return t.GetValue(t.Index);
	}
	public CharacterTrait GetTrait(Trait trait)
	{
		return trait switch
		{
			Trait.Speed => _speed,
			Trait.Might => _might,
			Trait.Sanity => _sanity,
			Trait.Knowledge => _knowledge,
			_ => null,
		};
	}
	
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
	}
}
