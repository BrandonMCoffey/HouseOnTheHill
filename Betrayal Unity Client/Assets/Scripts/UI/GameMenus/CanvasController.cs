using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Instance;

    [SerializeField] private PanelSwitcher _switcher;
	[SerializeField] private EnterRoomDisplay _enterRoomDisplay;
	[SerializeField] private CircleSlices _stepsTaken;
	
	[SerializeField] private EventPopup _eventPopup;
	
	public static Player LocalPlayer => Instance._manager.LocalPlayer;

	public static Action MenuStateChanged = delegate { };
	public static bool EventPopupOpen => Instance._switcher.CurrentlyOpenPanel == 2;
	public static bool PauseMenuOpen => Instance._switcher.CurrentlyOpenPanel == 3;
	public static bool InventoryOpen => Instance._switcher.CurrentlyOpenPanel == 4;

	private PlayerManager _manager;

    private void Awake()
    {
        Instance = this;
    }
    
	private void Start()
	{
		if (!_manager) _manager = FindObjectOfType<PlayerManager>();
	}
	
	public void SetPlayerManager(PlayerManager manager)
	{
		_manager = manager;
	}
	
	public static void OpenHud()
	{
		switch (GameController.Phase)
		{
		case GamePhase.ExplorationPhase:
			OpenExplorationHud();
			break;
		case GamePhase.EventPhase:
			OpenEventHud();
			break;
		case GamePhase.SpectatePhase:
			OpenSpectatorHud();
			break;
		}
	}
	
	public static void OpenExplorationHud()
	{
		Instance._switcher.OpenPanel(0);
        HideMouse(true);
	}

	public static void OpenEventHud()
	{
		Instance._switcher.OpenPanel(1);
		HideMouse(true);
	}
	
	public static void OpenEventPrompt(string header, string description)
	{
		Instance._switcher.OpenPanel(2);
		Instance._eventPopup.OpenPopup(header, description);
		HideMouse(false);
	}

    public static void OpenPauseMenu()
    {
	    Instance._switcher.OpenPanel(3);
        HideMouse(false);
    }

    public static void OpenInventory()
    {
	    Instance._switcher.OpenPanel(4);
        HideMouse(false);
    }
    
	public static void OpenSpectatorHud()
	{
		Instance._switcher.OpenPanel(5);
		HideMouse(false);
	}

    private static void HideMouse(bool hide)
    {
        Cursor.lockState = hide ? CursorLockMode.Locked : CursorLockMode.None;
	    Cursor.visible = !hide;
	    MenuStateChanged?.Invoke();
    }

    public void EndTurn()
    {
	    if (LocalUser.Instance) LocalUser.Instance.EndTurn();
	    else GameController.Instance.StartExplorationPhase();
    }
    
	public void QuitGame()
	{
		GameController.QuitGame();
	}
    
	public static void DisplayNewRoom(string roomName) => Instance._enterRoomDisplay.DisplayRoomName(roomName);
	public static void SetMaxSteps(int max) => Instance._stepsTaken.SetMax(max);
	public static void SetStepsTaken(int steps) => Instance._stepsTaken.SetFilled(steps);
}
