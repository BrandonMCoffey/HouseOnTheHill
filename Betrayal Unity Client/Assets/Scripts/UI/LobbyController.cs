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
		NetworkManager.OnDidDisconnect += OnDidDisconnect;
	}
	
	private void OnDisable()
	{
		NetworkManager.OnConnected -= OnConnected;
		NetworkManager.OnFailedConnection -= OnFailedConnection;
		NetworkManager.OnDidDisconnect -= OnDidDisconnect;
	}
	
	private void OnConnected()
	{
		_panelSwitcher.OpenPanel(1);
	}
	
	private void OnFailedConnection()
	{
		_panelSwitcher.OpenPanel(2);
	}
	
	private void OnDidDisconnect()
	{
		_panelSwitcher.OpenPanel(3);
	}
	
	public void RetryConnection()
	{
		_panelSwitcher.OpenPanel(0);
		NetworkManager.Instance.Connect();
	}
	
	public void QuitToMainMenu()
	{
		NetworkManager.Disconnect();
	}
	
	public void SetPlayerReady(bool ready)
	{
		LocalUser.SetReady(ready);
	}
}
