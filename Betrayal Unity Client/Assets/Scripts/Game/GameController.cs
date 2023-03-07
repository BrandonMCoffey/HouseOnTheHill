using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController Instance;
	
	[SerializeField] private GamePhase _startingPhase = GamePhase.ExplorationPhase;
	[SerializeField, ReadOnly] private GamePhase _phase = GamePhase.None;
	[SerializeField, ReadOnly] private int _stepsTaken;
	[SerializeField, ReadOnly] private int _maxSteps;
	
	[Header("References")]
	[SerializeField] private PlayerActionManager _player;
	[SerializeField] private SpectatorActionManager _spectator;
	[SerializeField] private RoomController _roomController;
	
	private void OnValidate()
	{
		if (!_player) _player = FindObjectOfType<PlayerActionManager>();
		if (!_spectator) _spectator = FindObjectOfType<SpectatorActionManager>();
		if (!_roomController) _roomController = FindObjectOfType<RoomController>();
	}
	
	private void Awake()
	{
		Instance = this;
	}
	
	private void Start()
    {
	    if (NetworkManager.Instance == null) SetPhase(_startingPhase);
    }
    
	private void SetLocalPlayerCurrentTurn()
	{
		StartExplorationPhase();
	}
	
	private void SetPhase(GamePhase phase)
	{
		switch (phase)
		{
		case GamePhase.ExplorationPhase:
		case GamePhase.EventPhase:
			StartExplorationPhase();
			break;
		case GamePhase.SpectatePhase:
			StartSpectatePhase();
			break;
		}
	}
    
	[Button]
	public void StartExplorationPhase()
	{
		if (_phase == GamePhase.ExplorationPhase) return;
		_phase = GamePhase.ExplorationPhase;
        
		_player.enabled = true;
		_player.SetPlayerEnabled(true);
		_roomController.SetShowRoomTops(true);
		_roomController.OpenAllConnectedDoors(true);
		_spectator.SetSpectatorEnabled(false);
		_spectator.enabled = false;
		
		_maxSteps = 5;
		CanvasController.SetMaxSteps(5); // TODO: Get Character Speed _maxSteps
		CanvasController.SetStepsTaken(_stepsTaken = -1); // Check first room?
	}
	
	public void StartEventPhase(DoorController door)
	{
		if (_phase == GamePhase.EventPhase) return;
		_phase = GamePhase.EventPhase;
		
		_roomController.OpenAllConnectedDoors(false);
		_player.PlayDoorOpenSequence(door);
	}
	
	[Button]
	public void StartSpectatePhase()
	{
		if (_phase == GamePhase.SpectatePhase) return;
		_phase = GamePhase.SpectatePhase;
		
		_player.SetPlayerEnabled(false);
		_roomController.SetShowRoomTops(false);
		_player.enabled = false;
		_spectator.enabled = true;
		_spectator.SetSpectatorEnabled(true);
	}
	
	public static void EnterNewRoom(Room room)
	{
		CanvasController.SetCurrentRoom(room.Name);
		Instance._stepsTaken++;
		Instance.CheckStepsTaken();
	}
	
	public void CheckStepsTaken()
	{
		CanvasController.SetStepsTaken(_stepsTaken);
		if (_stepsTaken >= _maxSteps)
		{
			LocalUser.Instance.EndTurn();
		}
	}
}

public enum GamePhase
{
	ExplorationPhase,
	EventPhase,
	SpectatePhase,
	None
}