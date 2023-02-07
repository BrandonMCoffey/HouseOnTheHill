using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 10;
    [SerializeField] private PlayerActionManager _actions;
    
    public static Vector3 MousePos => Mouse.current.position.ReadValue();
    private bool InGame => _actions.State == PlayerState.InGame;
    private bool Spectating => _actions.State == PlayerState.Spectating;
    
    private void OnMove(InputValue value)
    {
        if (InGame) _actions.Move(value.Get<Vector2>());
    }

    private void OnLook(InputValue value)
    {
        var look = value.Get<Vector2>() * _mouseSensitivity;
        if (InGame) _actions.Look(look);
        else if (Spectating) _actions.MouseMovement(look);
    }

    private void OnSprint(InputValue value)
    {
        _actions.Sprint(value.isPressed);
    }

    private void OnJump(InputValue value)
    {
        if (InGame) _actions.Jump(value.isPressed);
    }

    private void OnInteract(InputValue value)
    {
        switch (_actions.State)
        {
            case PlayerState.Spectating:
                _actions.Rotate(value.isPressed);
                break;
            case PlayerState.InGame:
                if (value.isPressed) _actions.Interact();
                break;
        }
    }

    private void OnPan(InputValue value)
    {
        if (Spectating) _actions.Pan(value.isPressed);
    }

    private void OnZoom(InputValue value)
    {
        if (Spectating) _actions.Zoom(value.Get<float>());
    }

    private void OnPauseGame()
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
                _actions.OpenPauseMenu();
                break;
            case PlayerState.InPauseMenu:
            case PlayerState.InInventory:
                _actions.CloseAnyMenu();
                break;
        }
    }

    private void OnOpenInventory()
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
                _actions.OpenInventory();
                break;
            case PlayerState.InInventory:
                _actions.CloseAnyMenu();
                break;
        }
    }
}
