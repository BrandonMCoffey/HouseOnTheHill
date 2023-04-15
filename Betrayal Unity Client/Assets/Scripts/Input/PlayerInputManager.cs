using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 10;
    
	public static Vector2 MoveDir { get; private set; }
	public static Vector2 LookDir { get; private set; }
	public static bool Sprint { get; private set; }
	public static bool Jump { get; private set; }
	public static Action<bool> Interact = delegate { };
	public static Action<bool> Pan = delegate { };
	public static Action<bool> LookAround = delegate { };
	public static Action<float> Zoom = delegate { };
	public static Action Pause = delegate { };
	public static Action OpenInventory = delegate { };
	public static Vector3 MousePos => Mouse.current.position.ReadValue();
    
    private void OnMove(InputValue value)
    {
	    MoveDir = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
	    LookDir = value.Get<Vector2>() * _mouseSensitivity;
    }

    private void OnSprint(InputValue value)
	{
		Sprint = value.isPressed;
    }

    private void OnJump(InputValue value)
	{
		Jump = value.isPressed;
    }

    private void OnInteract(InputValue value)
	{
		if (value.isPressed && GameController.Phase == GamePhase.EndTurnPhase) CanvasController.EndTurn();
		Interact?.Invoke(value.isPressed);
	}
	private void OnInteractSecondary(InputValue value)
	{
		Interact?.Invoke(value.isPressed);
	}

    private void OnPan(InputValue value)
	{
		Pan?.Invoke(value.isPressed);
	}

    private void OnZoom(InputValue value)
	{
		Zoom?.Invoke(value.Get<float>());
	}

	private void OnLookAround(InputValue value)
	{
		LookAround?.Invoke(value.isPressed);
	}

    private void OnPauseGame()
	{
		Pause?.Invoke();
    }

    private void OnOpenInventory()
	{
		OpenInventory?.Invoke();
    }
}
