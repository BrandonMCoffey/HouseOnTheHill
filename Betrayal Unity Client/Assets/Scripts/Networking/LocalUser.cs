using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUser : User
{
	public static LocalUser Instance;

	private GameController _gameController;
	
	private void Awake()
	{
		Instance = this;
		_gameController = FindObjectOfType<GameController>();
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
		if (currentTurn) _gameController.StartExplorationPhase();
		else _gameController.StartSpectatePhase();
	}

	public void EndTurn()
	{
		NetworkManager.OnLocalUserEndTurn();
	}
}
