using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Betrayal/Event")]
public class EventBase : ScriptableObject
{
	[Header("Data")]
	[SerializeField] private int _id;
	[SerializeField] private string _name;
	[SerializeField, TextArea] private string _description;
	
	[Header("Event")]
	[SerializeField] private bool _spawnPrefab;
	[SerializeField, ShowIf("_spawnPrefab")] private GameObject _prefab;
	
	public int Id => _id;
	public void SetId(int id) => _id = id;
	public string Name => _name;
	public string Description => _description;
	
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(_name)) _name = name;
	}
	
	public bool ActivateEvent()
	{
		if (_spawnPrefab)
		{
			var obj = Instantiate(_prefab);
			return true;
		}
		return false;
	}
}
