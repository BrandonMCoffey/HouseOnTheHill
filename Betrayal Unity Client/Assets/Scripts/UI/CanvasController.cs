using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Instance;

    [SerializeField] private PanelSwitcher _switcher;
	[SerializeField] private bool _spectating;
	[SerializeField] private TMP_Text _currentRoom;
	[SerializeField] private CircleSlices _stepsTaken;
	
	public static Player LocalPlayer => Instance._manager.LocalPlayer;

    public static bool PauseMenuOpen => Instance._switcher.CurrentlyOpenPanel == 1;
    public static bool InventoryOpen => Instance._switcher.CurrentlyOpenPanel == 2;

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

    public static void OpenPauseMenu()
    {
	    Instance._switcher.OpenPanel(2);
        HideMouse(false);
    }

    public static void OpenInventory()
    {
	    Instance._switcher.OpenPanel(3);
        HideMouse(false);
    }
    
	public static void OpenSpectatorMenu()
	{
		Instance._switcher.OpenPanel(4);
		HideMouse(false);
	}

    private static void HideMouse(bool hide)
    {
        Cursor.lockState = hide ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hide;
    }

    public void EndTurn()
    {
        LocalUser.Instance.EndTurn();
    }
    
	public static void SetCurrentRoom(string roomName) => Instance._currentRoom.text = roomName;
	public static void SetMaxSteps(int max) => Instance._stepsTaken.SetMax(max);
	public static void SetStepsTaken(int steps) => Instance._stepsTaken.SetFilled(steps);
}
