using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    None,
    Spectating,
    InGame,
    InPauseMenu,
    InInventory
}

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private bool _logAction;
    [SerializeField] private bool _logMovement;
    [SerializeField] private bool _logState;
    [SerializeField] private PlayerState _defaultState;
    [SerializeField, ReadOnly] private PlayerState _playerState = PlayerState.None;

    [Header("External References")]
    [SerializeField] private RoomController _roomController;
    
    [Header("Player References")]
    [SerializeField] private MovementController _firstPersonMovement;
    [SerializeField] private InteractionController _firstPersonInteraction;
    [SerializeField] private SpectatorMovement _spectatorMovement;
    
    public PlayerState State => _playerState;
    private bool InGame => State == PlayerState.InGame;
    private bool Spectating => State == PlayerState.Spectating;

    private void Awake()
    {
        _playerState = PlayerState.None;
        TrySetState(_defaultState);
    }

    [Button]
    public void SwitchToFirstPerson()
    {
        CloseAnyMenu();
        TrySetState(PlayerState.InGame);
    }

    [Button]
    public void SwitchToSpectator()
    {
        CloseAnyMenu();
        TrySetState(PlayerState.Spectating);
    }

    public void Move(Vector2 moveDir)
    {
        LogMovement("Move: " + moveDir);
        _firstPersonMovement.SetMoveDir(moveDir);
    }

    public void Look(Vector2 look)
    {
        LogMovement("Look: " + look);
        _firstPersonMovement.SetLookDir(look);
    }

    public void Sprint(bool sprint)
    {
        LogAction($"Sprinting: {sprint}");
        _firstPersonMovement.SetSprinting(sprint);
    }

    public void Jump(bool jump)
    {
        LogAction("Jump");
        _firstPersonMovement.SetJumpThisFrame();
    }
    
    public void Interact()
    {
        LogAction("Interact");
        _firstPersonInteraction.Interact();
    }

    public void Rotate(bool rotate)
    {
        LogAction("Rotate: " + rotate);
        _spectatorMovement.SetRotate(rotate);
    }

    public void Pan(bool pan)
    {
        LogAction("Pan: " + pan);
        _spectatorMovement.SetPan(pan);
    }

    public void MouseMovement(Vector2 mouseMovement)
    {
        LogMovement("Mouse Movement: " + mouseMovement);
        _spectatorMovement.SetMouseMovement(mouseMovement);
    }

    public void Zoom(float zoom)
    {
        LogMovement("Zoom: " + zoom);
        _spectatorMovement.Zoom(zoom);
    }
    
    public void OpenPauseMenu()
    {
        LogAction("Open Pause Menu");
        CanvasController.OpenPauseMenu();
        CheckUiState();
    }

    public void OpenInventory()
    {
        LogAction("Open Inventory");
        CanvasController.OpenInventory();
        CheckUiState();
    }

    public void CloseAnyMenu()
    {
        LogAction("Close Any Menu");
        CanvasController.CloseMenu();
        CheckUiState();
    }

    private void CheckUiState()
    {
        if (CanvasController.PauseMenuOpen) TrySetState(PlayerState.InPauseMenu);
        else if (CanvasController.InventoryOpen) TrySetState(PlayerState.InInventory);
        else TrySetState(PlayerState.InGame);
    }
    
    private bool TrySetState(PlayerState newState)
    {
        if (newState == State) return false;
        if (_logState) Debug.Log($"Player State switched to {newState}", gameObject);
        _playerState = newState;
        
        _firstPersonMovement.SetCanMove(InGame);
        _firstPersonInteraction.SetCameraActive(!Spectating);
        _spectatorMovement.SetCameraActive(Spectating);
        
        _roomController.SetShowRoomTops(!Spectating);
        
        return true;
    }

    private void LogMovement(string message)
    {
        if (_logMovement) Debug.Log(message, gameObject);
    }

    private void LogAction(string message)
    {
        if (_logAction) Debug.Log(message, gameObject);
    }
}
