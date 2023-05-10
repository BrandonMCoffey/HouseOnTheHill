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
	[SerializeField] private bool _omen;
	[SerializeField] private ItemType _type;
	[SerializeField] private string _name;
	[SerializeField, TextArea] private string _description;

	[Header("References")]
	[SerializeField] private Sprite _icon;
	[SerializeField] private GameObject _prefab;
	
	[Header("Effects")]
	[SerializeField, TextArea] private string _effectsDescription;
	
	public int Id => _id;
	public void SetId(int id) => _id = id;
	public bool Omen => _omen;
	public ItemType Type => _type;
	public string Name => _name;
	public string Description => _description;
	public Sprite IconSprite => _icon;
	public GameObject Prefab => _prefab;
	public string EffectsDescription => _effectsDescription;
	
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
	}
}
