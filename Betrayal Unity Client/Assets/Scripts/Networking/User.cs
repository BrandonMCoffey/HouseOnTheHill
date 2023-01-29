using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
	[SerializeField, ReadOnly] private ushort _id;
	[SerializeField, ReadOnly] private bool _local;
	[SerializeField, ReadOnly] private string _name;
	[SerializeField, ReadOnly] private bool _ready;
	[SerializeField, ReadOnly] private int _character = -1;
	
	public bool IsLocal => _local;
	public string UserName => _name;
	public bool Ready => _ready;
	public string CharacterName => _character >= 0 ? GameState.GetCharacter(_character).Name : "Spectator";
	public bool IsPlayer => _character >= 0;
	
	public static Action OnUpdateAllUsers = delegate { };
	public static List<User> AllUsers = new List<User>();
	
	public static Action OnUpdatePlayerStates = delegate { };
	public static List<int> RemoteCharacters = new List<int>();
	
	private void Start()
	{
		AllUsers.Add(this);
		OnUpdateAllUsers?.Invoke();
	}
	
	private void OnDestroy()
	{
		AllUsers.Remove(this);
		OnUpdateAllUsers?.Invoke();
	}
	
	public void CreateUser(ushort id, bool local, string name)
	{
		_id = id;
		_local = local;
		_name = name;
		gameObject.name = $"{(local ? "Local" : "Remote")} User ({_name})";
		OnUpdatePlayerStates?.Invoke();
	}
	
	public void DestroyUser()
	{
		if (!_local) 
		{
			RemoteCharacters.Remove(_character);
			OnUpdatePlayerStates?.Invoke();
		}
		Destroy(gameObject);
	}
	
	public void SetCharacter(int character)
	{
		if (!_local) RemoteCharacters.Remove(_character);
		_character = character;
		if (!_local) RemoteCharacters.Add(_character);
		OnUpdatePlayerStates?.Invoke();
	}
	
	public void SetReady(bool ready)
	{
		_ready = ready;
		OnUpdatePlayerStates?.Invoke();
	}
}
