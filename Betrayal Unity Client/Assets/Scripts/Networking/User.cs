using System;
using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

public class User : MonoBehaviour
{
	[Header("Constant Data")]
	[SerializeField, ReadOnly] protected ushort _id;
	[SerializeField, ReadOnly] protected bool _local;
	[SerializeField, ReadOnly] protected string _name;
	
	[Header("Lobby Data")]
	[SerializeField, ReadOnly] protected bool _ready;
	[SerializeField, ReadOnly] protected int _character = -2;
	
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
		_ready = false;
		_character = -10;
		gameObject.name = $"{(local ? "Local" : "Remote")} User ({_name})";
		OnUpdatePlayerStates?.Invoke();
	}
	
	public void DestroyUser()
	{
		if (!_local) 
		{
			if (_character >= 0) RemoteCharacters.Remove(_character);
			OnUpdatePlayerStates?.Invoke();
		}
		Destroy(gameObject);
	}
	
	public virtual void SetCharacter(int character)
	{
		if (!_local && _character >= 0) RemoteCharacters.Remove(_character);
		_character = character;
		if (!_local && _character >= 0) RemoteCharacters.Add(_character);
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
	
	public virtual void SetTransform(Vector3 pos, Vector3 rot, Vector3 cameraRot, bool updatePlayer = true)
	{
		if (updatePlayer && _player) _player.SetPositionAndRotationValues(pos, Quaternion.Euler(rot), Quaternion.Euler(cameraRot));
	}

	public virtual void SetCurrentTurn(bool currentTurn) => _isCurrentTurn = currentTurn;
}
