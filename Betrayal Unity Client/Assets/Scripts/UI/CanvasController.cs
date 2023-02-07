using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Instance;

    [SerializeField] private PanelSwitcher _switcher;

    public static bool PauseMenuOpen => Instance._switcher.CurrentlyOpenPanel == 1;
    public static bool InventoryOpen => Instance._switcher.CurrentlyOpenPanel == 2;

    private void Awake()
    {
        Instance = this;
    }

    public static void CloseMenu()
    {
        Instance._switcher.OpenPanel(0);
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
