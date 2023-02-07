using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUser : User
{
	public static LocalUser Instance;

	private PlayerManager _playerManager;
	
	private void Awake()
	{
		Instance = this;
	}

	public void SetPlayerManager(PlayerManager manager)
	{
		_playerManager = manager;
	}
	
	public override void SetCharacter(int character)
	{
		base.SetCharacter(character);
		NetworkManager.OnLocalUserSelectCharacter(character);
	}
	
	public override void SetReady(bool ready)
	{
		base.SetReady(ready);
		NetworkManager.OnLocalUserReadyUp(ready);
	}
	
	public override void SetTransform(Vector3 pos, Vector3 rot, bool updatePlayer = true)
	{
		base.SetTransform(pos, rot, false);
		NetworkManager.OnUpdateLocalUserTransformCharacter(pos, rot);
	}

	public override void SetCurrentTurn(bool currentTurn)
	{
		base.SetCurrentTurn(currentTurn);
		if (currentTurn) _playerManager.SwitchToFirstPerson();
		else _playerManager.SwitchToSpectator();
	}

	public void EndTurn()
	{
		NetworkManager.OnLocalUserEndTurn();
	}
}
