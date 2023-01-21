using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
	[SerializeField] private PanelSwitcher _panelSwitcher;
	
	private void OnEnable()
	{
		NetworkManager.OnConnected += OnConnected;
		NetworkManager.OnFailedConnection += OnFailedConnection;
	}
	
	private void OnDisable()
	{
		NetworkManager.OnConnected -= OnConnected;
		NetworkManager.OnFailedConnection -= OnFailedConnection;
	}
	
	private void OnConnected()
	{
		_panelSwitcher.OpenPanel(1);
	}
	
	private void OnFailedConnection()
	{
		_panelSwitcher.OpenPanel(2);
	}
	
	public void QuitToMainMenu()
	{
		NetworkManager.Disconnect();
	}
}
