using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UiState
{
	Hud,
	EventPopup,
	ItemPopup,
	Inventory,
	Pause,
}

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private Player _localPlayer;
	[SerializeField] private Player _remotePlayerPrefab;
	
	[SerializeField] private bool _logUiActions;
	[SerializeField] private UiState _uiState = UiState.Hud;
	[SerializeField, ReadOnly] private bool _ignoreInput;

	public static bool MenuOpen;
	public static Action OnPlayersLoaded = delegate { };
	public static List<Player> Players;
	
	public Player LocalPlayer => _localPlayer;
	
	private void OnEnable()
	{
		GameController.OnUpdatePhase += CanvasController.OpenHud;
		CanvasController.MenuStateChanged += CheckUiState;
		PlayerInputManager.Pause += OpenPauseMenu;
		PlayerInputManager.OpenInventory += OpenInventory;
	}
	
	private void OnDisable()
	{
		GameController.OnUpdatePhase -= CanvasController.OpenHud;
		CanvasController.MenuStateChanged -= CheckUiState;
		PlayerInputManager.Pause -= OpenPauseMenu;
		PlayerInputManager.OpenInventory -= OpenInventory;
	}
	
	private void Start()
	{
		Players = new List<Player>();
		if (CanvasController.Instance) CanvasController.Instance.SetPlayerManager(this);
		if (!NetworkManager.Instance) return;
		NetworkManager.OnGameLoaded();
		bool localPlayerExists = false;
		foreach (var pair in NetworkManager.AllUsers)
		{
			var user = pair.Value;
			if (user.Character > 0)
			{
				bool isLocal = user.IsLocal;
				localPlayerExists |= isLocal;
				var player = isLocal ? _localPlayer : Instantiate(_remotePlayerPrefab, transform);
				user.SetPlayer(player);
				player.SetUser(user);
				Players.Add(player);
				if (user.IsLocal) _localPlayer = player;
			}
		}
		if (!localPlayerExists) _localPlayer.gameObject.SetActive(false);
		OnPlayersLoaded?.Invoke();
	}
	
	public void SetIgnoreInput(bool ignoreInput) => _ignoreInput = ignoreInput;
	
	public void OpenPauseMenu()
	{
		if (_ignoreInput) return;
		if (CanvasController.PauseMenuOpen)
		{
			CloseAnyMenu();
			return;
		}
		LogAction("Open Pause Menu");
		CanvasController.OpenPauseMenu();
	}

	public void OpenInventory()
	{
		if (_ignoreInput) return;
		if (CanvasController.InventoryOpen)
		{
			CloseAnyMenu();
			return;
		}
		LogAction("Open Inventory");
		CanvasController.OpenInventory();
	}

	public void CloseAnyMenu()
	{
		if (_ignoreInput) return;
		LogAction("Close Any Menu");
		CanvasController.OpenHud();
	}

	private void CheckUiState()
	{
		if (CanvasController.EventPopupOpen) _uiState = UiState.EventPopup;
		else if (CanvasController.ItemPopupOpen) _uiState = UiState.ItemPopup;
		else if (CanvasController.PauseMenuOpen) _uiState = UiState.Pause;
		else if (CanvasController.InventoryOpen) _uiState = UiState.Inventory;
		else _uiState = UiState.Hud;
		MenuOpen = _uiState != UiState.Hud;
	}

	private void LogAction(string message)
	{
		if (_logUiActions) Debug.Log(message, gameObject);
	}
}
