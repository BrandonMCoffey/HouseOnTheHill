using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorActionManager : MonoBehaviour
{
	[SerializeField] private bool _logAction;
	[SerializeField] private SpectatorMovement _spectatorMovement;
	
	private void Update()
	{
		_spectatorMovement.SetMouseMovement(PlayerInputManager.LookDir);
	}

	public void SetSpectatorEnabled(bool active)
	{
		_spectatorMovement.SetCameraActive(active);
		CanvasController.OpenSpectatorMenu();
	}
	
	private void OnEnable()
	{
		PlayerInputManager.Interact += Rotate;
		PlayerInputManager.Pan += Pan;
		PlayerInputManager.Zoom += Zoom;
		PlayerInputManager.LookAround += LookAround;
	}
	
	private void OnDisable()
	{
		PlayerInputManager.Interact -= Rotate;
		PlayerInputManager.Pan -= Pan;
		PlayerInputManager.Zoom -= Zoom;
		PlayerInputManager.LookAround += LookAround;
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

	public void Zoom(float zoom)
	{
		LogAction("Zoom: " + zoom);
		_spectatorMovement.Zoom(zoom);
	}

	public void LookAround(bool lookAround)
	{
		LogAction("Look Around: " + lookAround);
		_spectatorMovement.SetLookAround(lookAround);
	}

	private void LogAction(string message)
	{
		if (_logAction) Debug.Log(message, gameObject);
	}
}
