using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUser : MonoBehaviour
{
	[SerializeField, ReadOnly] private ushort _id;
	[SerializeField, ReadOnly] private string _name;
	[SerializeField, ReadOnly] private string _character;
	
	public void CreateUser(ushort id, string name)
	{
		_id = id;
		_name = name;
		gameObject.name = $"Local User ({name})";
		NetworkManager.OnLocalUserCreated(name);
	}
	
	public void DestroyUser()
	{
		Destroy(gameObject);
	}
	
	public void SetCharacter(string character)
	{
		_character = character;
		NetworkManager.OnLocalUserSelectCharacter(character);
	}
}
