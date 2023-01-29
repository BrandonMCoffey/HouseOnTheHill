using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
	public static Action<float> StartCountdown = delegate { };
	public static Action StopCountdown = delegate { };
	
	[SerializeField] private PanelSwitcher _panelSwitcher;
	[SerializeField] private CountdownTimer _countdown;
	
	private void OnEnable()
	{
		NetworkManager.OnConnected += OnConnected;
		NetworkManager.OnFailedConnection += OnFailedConnection;
		NetworkManager.OnDidDisconnect += OnDidDisconnect;
		StartCountdown += OnStartCountdown;
		StopCountdown += OnStopCountdown;
	}
	
	private void OnDisable()
	{
		NetworkManager.OnConnected -= OnConnected;
		NetworkManager.OnFailedConnection -= OnFailedConnection;
		NetworkManager.OnDidDisconnect -= OnDidDisconnect;
		StartCountdown -= OnStartCountdown;
		StopCountdown -= OnStopCountdown;
	}
	
	private void OnConnected() => _panelSwitcher.OpenPanel(1);
	private void OnFailedConnection() => _panelSwitcher.OpenPanel(2);
	private void OnDidDisconnect() => _panelSwitcher.OpenPanel(3);
	
	private void OnStartCountdown(float length) => _countdown.StartTimer(length);
	private void OnStopCountdown() => _countdown.StopTimer();
	
	public void RetryConnection()
	{
		_panelSwitcher.OpenPanel(0);
		NetworkManager.Instance.Connect();
	}
	
	public void QuitToMainMenu() => NetworkManager.Disconnect();
	public void SetPlayerReady(bool ready) => LocalUser.SetReady(ready);
}
