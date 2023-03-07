using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    None,
	Exploration,
	Event,
    InPauseMenu,
    InInventory
}

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private bool _logAction;
    [SerializeField] private bool _logState;
    [SerializeField, ReadOnly] private PlayerState _playerState = PlayerState.None;

    [Header("External References")]
    [SerializeField] private RoomController _roomController;
    
	[Header("Player References")]
    [SerializeField] private MovementController _firstPersonMovement;
    [SerializeField] private InteractionController _firstPersonInteraction;
	[SerializeField] private DoorOpenSequence _doorOpenSequence;
    
	private bool _exploration = true;
	private bool InGame => _playerState == PlayerState.Exploration || _playerState == PlayerState.Event;

	public Transform CameraParent => _firstPersonMovement.CameraParent;

    private void Awake()
    {
        _playerState = PlayerState.None;
    }
    
	private void Update()
	{
		if (InGame)
		{
			_firstPersonMovement.ProcessMovement();
		}
	}
	
	private void OnEnable()
	{
		PlayerInputManager.Interact += Interact;
		PlayerInputManager.Pause += OpenPauseMenu;
		PlayerInputManager.OpenInventory += OpenInventory;
	}
	
	private void OnDisable()
	{
		PlayerInputManager.Interact -= Interact;
		PlayerInputManager.Pause -= OpenPauseMenu;
		PlayerInputManager.OpenInventory -= OpenInventory;
	}

	public void DisablePlayerMovement() => _firstPersonMovement.SetCanMove(false);
	public void SetPlayerEnabled(bool active, bool exploration = true)
	{
		CloseAnyMenu();
		TrySetState(active ? (exploration ? PlayerState.Exploration : PlayerState.Event) : PlayerState.None);
        
		_exploration = exploration;
		if (exploration) CanvasController.OpenExplorationHud();
		else CanvasController.OpenEventHud();
		
		_firstPersonMovement.SetCanMove(active);
		
		_firstPersonInteraction.SetCameraActive(active);
		_firstPersonInteraction.SetCanOpenDoor(exploration);
	}
	
	public void PlayDoorOpenSequence(DoorController door)
	{
		_doorOpenSequence.PlaySequence(door, this);
	}
    
	private void Interact(bool interact)
	{
		if (!interact || !InGame) return;
		LogAction("Interact");
		_firstPersonInteraction.Interact();
	}
    
    public void OpenPauseMenu()
	{
		if (CanvasController.PauseMenuOpen)
		{
			CloseAnyMenu();
			return;
		}
        LogAction("Open Pause Menu");
        CanvasController.OpenPauseMenu();
        CheckUiState();
    }

    public void OpenInventory()
	{
		if (CanvasController.InventoryOpen)
		{
			CloseAnyMenu();
			return;
		}
        LogAction("Open Inventory");
        CanvasController.OpenInventory();
        CheckUiState();
    }

    public void CloseAnyMenu()
    {
	    LogAction("Close Any Menu");
	    if (_exploration) CanvasController.OpenExplorationHud();
	    else CanvasController.OpenEventHud();
        CheckUiState();
    }

    private void CheckUiState()
    {
        if (CanvasController.PauseMenuOpen) TrySetState(PlayerState.InPauseMenu);
        else if (CanvasController.InventoryOpen) TrySetState(PlayerState.InInventory);
        else TrySetState(_exploration ? PlayerState.Exploration : PlayerState.Event);
    }
    
    private bool TrySetState(PlayerState newState)
    {
	    if (newState == _playerState) return false;
        if (_logState) Debug.Log($"Player State switched to {newState}", gameObject);
        _playerState = newState;
        
        return true;
    }

    private void LogAction(string message)
    {
        if (_logAction) Debug.Log(message, gameObject);
    }
}
