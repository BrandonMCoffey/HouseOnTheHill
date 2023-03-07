using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private Player _localPlayer;
	[SerializeField] private PlayerActionManager _localPlayerActions;
	[SerializeField] private Player _remotePlayerPrefab;

	public static Action OnPlayersLoaded = delegate { };
	public static List<Player> Players;
	
	public Player LocalPlayer => _localPlayer;
	
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
}
