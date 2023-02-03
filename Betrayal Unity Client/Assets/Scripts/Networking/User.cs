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
	[SerializeField, ReadOnly] private Player _player;
	
	public bool IsLocal => _local;
	public string UserName => _name;
	public bool Ready => _ready;
	public int Character => _character;
	
	public static Action OnUpdatePlayerStates = delegate { };
	public static List<int> RemoteCharacters = new List<int>();
	
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
		
		if (GameState.GameStarted)
		{
			foreach (var player in PlayerManager.Players)
			{
				if (player.Character == GameState.GetCharacter(_character))
					_player = player;
			}
		}
	}
	
	public void SetReady(bool ready)
	{
		_ready = ready;
		OnUpdatePlayerStates?.Invoke();
	}
	
	public void SetPlayer(Player player)
	{
		_player = player;
		_player.SetCharacter(GameState.GetCharacter(_character));
	}
	
	public void SetTransform(Vector3 pos, Vector3 rot, bool updatePlayer = true)
	{
		if (updatePlayer) _player.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
	}
}
