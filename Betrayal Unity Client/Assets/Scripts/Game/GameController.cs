using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController Instance;
	
	[SerializeField] private GamePhase _startingPhase = GamePhase.ExplorationPhase;
	[SerializeField, ReadOnly] private GamePhase _phase = GamePhase.None;
	[SerializeField, ReadOnly] private int _itemsToCollect;
	[SerializeField, ReadOnly] private int _stepsTaken;
	[SerializeField, ReadOnly] private int _maxSteps = 8;
	[SerializeField, ReadOnly] private Room _currentRoom;
	
	[Header("References")]
	[SerializeField] private PlayerActionManager _player;
	[SerializeField] private SpectatorActionManager _spectator;
	[SerializeField] private RoomController _roomController;
	
	public static GamePhase Phase => Instance._phase;
	public static Action OnUpdatePhase = delegate { };
	public static Action OnUpdateObjectives = delegate { };
	public static bool CurrentTurn => Phase == GamePhase.ExplorationPhase || Phase == GamePhase.EventPhase || Phase == GamePhase.EndTurnPhase;
	public static Action<string> UpdateCurrentRoom = delegate { };
	public static string CurrentRoomName => Instance._currentRoom ? Instance._currentRoom.Name : "";
	
	private bool _endTurn;
	
	private void OnValidate()
	{
		if (!_player) _player = FindObjectOfType<PlayerActionManager>();
		if (!_spectator) _spectator = FindObjectOfType<SpectatorActionManager>();
		if (!_roomController) _roomController = FindObjectOfType<RoomController>();
	}
	
	private void Awake()
	{
		Instance = this;
		_maxSteps = 8;
	}
	
	private void OnEnable()
	{
		EventController.OnUpdateItemsToCollect += CheckItemsToCollect;
	}
	
	private void OnDisable()
	{
		EventController.OnUpdateItemsToCollect -= CheckItemsToCollect;
	}
	
	private void Start()
    {
	    if (NetworkManager.Instance == null)
	    {
		    switch (_startingPhase)
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
    }
    
	private void Update()
	{
		if (_endTurn && CanvasController.HudOpen)
		{
			StartEndTurnPhase();
			_endTurn = false;
		}
	}
    
	private void SetLocalPlayerCurrentTurn()
	{
		StartExplorationPhase();
	}
	
	[Button(Mode = ButtonMode.InPlayMode)]
	public void StartExplorationPhase()
	{
		if (!TrySetPhase(GamePhase.ExplorationPhase)) return;
        
		_player.enabled = true;
		_player.SetPlayerEnabled(true);
		_roomController.SetShowRoomTops(true);
		_roomController.OpenAllConnectedDoors(true);
		_spectator.SetSpectatorEnabled(false);
		_spectator.enabled = false;
		
		_maxSteps = 5;
		CanvasController.SetMaxSteps(5); // TODO: Get Character Speed _maxSteps
		CanvasController.SetStepsTaken(_stepsTaken = -1); // Check first room?
		CanvasController.OpenExplorationHud();
	}
	
	public void StartEventPhase(DoorController door)
	{
		if (!TrySetPhase(GamePhase.EventPhase)) return;
		
		_roomController.OpenAllConnectedDoors(false);
		_player.PlayDoorOpenSequence(door);
		CanvasController.OpenEventHud();
	}
	
	public List<string> GetObjectives()
	{
		List<string> list = new List<string>();
		switch (Phase)
		{
		case GamePhase.ExplorationPhase:
			list.Add("Explore a New Room");
			break;
		case GamePhase.EventPhase:
			if (_itemsToCollect > 1) list.Add($"Pickup {_itemsToCollect} Items");
			else if (_itemsToCollect == 1) list.Add($"Pickup 1 Item");
			else list.Add("Explore the Room");
			//list.Add("Make a Haunt Roll");
			break;
		case GamePhase.SpectatePhase:
			list.Add("Watch Active Player");
			list.Add("Plan your next move");
			break;
		case GamePhase.HauntPhase:
			list.Add("Escape! Get to the Lobby");
			break;
		}
		return list;
	}
	
	private void CheckItemsToCollect(int itemsToCollect)
	{
		_itemsToCollect = itemsToCollect;
		OnUpdateObjectives?.Invoke();
		CheckCanEndTurn();
	}
	
	public void CheckCanEndTurn()
	{
		if (_itemsToCollect == 0) _endTurn = true;
	}

	public void StartEndTurnPhase()
	{
		if (!TrySetPhase(GamePhase.EndTurnPhase)) return;
		
		CanvasController.OpenEndTurnHud();
	}
	
	[Button(Mode = ButtonMode.InPlayMode)]
	public void StartSpectatePhase()
	{
		if (!TrySetPhase(GamePhase.SpectatePhase)) return;
		
		_player.SetPlayerEnabled(false);
		_player.enabled = false;
		_roomController.SetShowRoomTops(false);
		_roomController.OpenAllConnectedDoors(true);
		_spectator.enabled = true;
		_spectator.SetSpectatorEnabled(true);
	}
	
	public static void EnterNewRoom(Room room)
	{
		Instance._currentRoom = room;
		UpdateCurrentRoom?.Invoke(room.Name);
		Instance._stepsTaken++;
		Instance.CheckStepsTaken();
	}
	
	private bool TrySetPhase(GamePhase phase)
	{
		if (_phase == phase) return false;
		_phase = phase;
		OnUpdatePhase?.Invoke();
		OnUpdateObjectives?.Invoke();
		Debug.Log("Game Phase: " + phase.ToString(), gameObject);
		return true;
	}
	
	public void CheckStepsTaken()
	{
		if (_phase != GamePhase.ExplorationPhase) return;
		CanvasController.SetStepsTaken(_stepsTaken);
		if (_stepsTaken >= _maxSteps)
		{
			EndTurn();
		}
	}
	
	public static void EndTurn()
	{
		if (LocalUser.Instance) LocalUser.Instance.EndTurn();
		else Instance.StartExplorationPhase();
	}
	
	public static void QuitGame()
	{
		if (NetworkManager.Instance) NetworkManager.Disconnect();
		else UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}

public enum GamePhase
{
	ExplorationPhase,
	EventPhase,
	HauntPhase,
	EndTurnPhase,
	SpectatePhase,
	None
}