using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
	[Header("Constant Data")]
	[SerializeField, ReadOnly] protected ushort _id;
	[SerializeField, ReadOnly] protected bool _local;
	[SerializeField, ReadOnly] protected string _name;
	
	[Header("Lobby Data")]
	[SerializeField, ReadOnly] protected bool _ready;
	[SerializeField, ReadOnly] protected int _character = -1;
	
	[Header("Game Data")]
	[SerializeField, ReadOnly] protected bool _isCurrentTurn;
	[SerializeField, ReadOnly] protected Player _player;
	
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
	
	public virtual void SetCharacter(int character)
	{
		if (!_local) RemoteCharacters.Remove(_character);
		_character = character;
		if (!_local) RemoteCharacters.Add(_character);
		OnUpdatePlayerStates?.Invoke();
		
		if (GameData.GameStarted)
		{
			foreach (var player in PlayerManager.Players)
			{
				if (player.Character == GameData.GetCharacter(_character))
					_player = player;
			}
		}
	}
	
	public virtual void SetReady(bool ready)
	{
		_ready = ready;
		OnUpdatePlayerStates?.Invoke();
	}
	
	public virtual void SetPlayer(Player player)
	{
		_player = player;
		_player.SetCharacter(GameData.GetCharacter(_character));
	}
	
	public virtual void SetTransform(Vector3 pos, Vector3 rot, bool updatePlayer = true)
	{
		if (updatePlayer && _player) _player.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
	}

	public virtual void SetCurrentTurn(bool currentTurn) => _isCurrentTurn = currentTurn;
}
