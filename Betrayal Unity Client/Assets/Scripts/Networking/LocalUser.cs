using UnityEngine;

public class LocalUser : User
{
	public static LocalUser Instance;
	
	private void Awake()
	{
		Instance = this;
	}

	public void OnChoosingCharacter()
	{
		if (Character == -10) SetCharacter(-9);
		SetReady(false);
	}

	public void StopChoosingCharacter()
	{
		if (Character == -9) SetCharacter(-10);
	}
	
	public override void SetCharacter(int character)
	{
		base.SetCharacter(character);
		NetworkManager.OnLocalUserSelectCharacter(character);
		if (character == -1) SetReady(true);
	}
	
	public void ToggleReady() => SetReady(!Ready);
	public override void SetReady(bool ready)
	{
		base.SetReady(ready);
		NetworkManager.OnLocalUserReadyUp(ready);
	}
	
	public override void SetTransform(Vector3 pos, Vector3 rot, Vector3 cameraRot, bool updatePlayer = true)
	{
		base.SetTransform(pos, rot, cameraRot, false);
		NetworkManager.OnUpdateLocalUserTransformCharacter(pos, rot, cameraRot);
	}

	public override void SetCurrentTurn(bool currentTurn)
	{
		base.SetCurrentTurn(currentTurn);
		if (currentTurn) GameController.Instance.StartExplorationPhase();
		else GameController.Instance.StartSpectatePhase();
	}

	public void EndTurn()
	{
		NetworkManager.OnLocalUserEndTurn();
	}
}
