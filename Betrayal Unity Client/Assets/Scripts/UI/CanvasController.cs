using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Instance;

    [SerializeField] private PanelSwitcher _switcher;
	[SerializeField] private Player _localPlayer;
	[SerializeField] private bool _spectating;
	
	public static Player LocalPlayer => Instance._localPlayer;

    public static bool PauseMenuOpen => Instance._switcher.CurrentlyOpenPanel == 1;
    public static bool InventoryOpen => Instance._switcher.CurrentlyOpenPanel == 2;

    private void Awake()
    {
        Instance = this;
    }
    
	private void OnEnable()
	{
		PlayerManager.OnPlayersLoaded += SetLocalPlayer;
	}
	
	private void OnDisable()
	{
		PlayerManager.OnPlayersLoaded -= SetLocalPlayer;
	}
	
	[Button]
	private void SetLocalPlayer()
	{
		foreach (var player in PlayerManager.Players)
		{
			if (player.IsLocal)
			{
				_localPlayer = player;
				return;
			}
		}
	}
	
	private void Update()
	{
		if (_spectating != PlayerActionManager.IsSpectating)
		{
			_spectating = PlayerActionManager.IsSpectating;
			if (_switcher.CurrentlyOpenPanel == 0 || _switcher.CurrentlyOpenPanel == 3)
			{
				Instance._switcher.OpenPanel(PlayerActionManager.IsSpectating ? 3 : 0);
			}
		}
	}

    public static void CloseMenu()
	{
		Instance._switcher.OpenPanel(PlayerActionManager.IsSpectating ? 3 : 0);
        HideMouse(true);
    }

    public static void OpenPauseMenu()
    {
        Instance._switcher.OpenPanel(1);
        HideMouse(false);
    }

    public static void OpenInventory()
    {
        Instance._switcher.OpenPanel(2);
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
}
