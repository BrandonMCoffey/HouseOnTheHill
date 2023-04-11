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
	[SerializeField] private ItemPickupDisplay _itemPopup;
	[SerializeField] private PopupBase _inventoryPopup;
	
	public static Player LocalPlayer => Instance._manager.LocalPlayer;

	public static Action MenuStateChanged = delegate { };
	public static bool EventPopupOpen => Instance._switcher.CurrentlyOpenPanel == 2;
	public static bool ItemPopupOpen => Instance._switcher.CurrentlyOpenPanel == 3;
	public static bool InventoryOpen => Instance._switcher.CurrentlyOpenPanel == 4;
	public static bool PauseMenuOpen => Instance._switcher.CurrentlyOpenPanel == 7;

	private PlayerManager _manager;
	private Coroutine _routine;

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
	
	public static void OpenExplorationHud() => Instance.AttemptOpenPanel(0, true);
	public static void OpenEventHud() => Instance.AttemptOpenPanel(1, true);
	public static void OpenEventPrompt(string header, string description)
	{
		Instance._eventPopup.SetValues(header, description);
		Instance.AttemptOpenPanel(2, false);
	}
	public static void OpenItemPrompt(Item item)
	{
		Instance._itemPopup.SetValues(item);
		Instance.AttemptOpenPanel(3, false);
	}
	public static void OpenInventory() => Instance.AttemptOpenPanel(4, false);
	public static void OpenEndTurnHud() => Instance.AttemptOpenPanel(5, true);
	public static void OpenSpectatorHud() => Instance.AttemptOpenPanel(6, false);
	public static void OpenPauseMenu() => Instance.AttemptOpenPanel(7, false);
	
	private void AttemptOpenPanel(int panelIndex, bool hideMouse)
	{
		Debug.Log(panelIndex);
		HideMouse(hideMouse);
		if (_routine != null) StopCoroutine(_routine);
		if (EventPopupOpen) _routine = StartCoroutine(ClosePopupDelay(_eventPopup, panelIndex));
		else if (ItemPopupOpen) _routine = StartCoroutine(ClosePopupDelay(_itemPopup, panelIndex));
		else if (InventoryOpen) _routine = StartCoroutine(ClosePopupDelay(_inventoryPopup, panelIndex));
		else OpenPanel(panelIndex);
	}
	
	private IEnumerator ClosePopupDelay(PopupBase closePopup, int panelIndex)
	{
		closePopup.ClosePopup();
		yield return new WaitForSeconds(closePopup.CloseTime);
		OpenPanel(panelIndex);
		_routine = null;
	}
	
	private void OpenPanel(int panelIndex)
	{
		_switcher.OpenPanel(panelIndex);
		
		if (EventPopupOpen) _eventPopup.OpenPopup();
		else if (ItemPopupOpen) _itemPopup.OpenPopup();
		else if (InventoryOpen) _inventoryPopup.OpenPopup();
		
		MenuStateChanged?.Invoke();
	}

    private static void HideMouse(bool hide)
    {
        Cursor.lockState = hide ? CursorLockMode.Locked : CursorLockMode.None;
	    Cursor.visible = !hide;
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
