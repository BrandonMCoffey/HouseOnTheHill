using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private bool _forceFirstPersonMode;
	[SerializeField] private Player _localPlayer;
	[SerializeField] private PlayerActionManager _localPlayerActions;
	[SerializeField] private Player _remotePlayerPrefab;

	public static System.Action OnPlayersLoaded = delegate { };
	public static List<Player> Players;
	
	private void Start()
	{
		Players = new List<Player>();
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
		if (_forceFirstPersonMode) _localPlayerActions.SwitchToFirstPerson();
		else SwitchToSpectator();
		if (!localPlayerExists) _localPlayer.gameObject.SetActive(false);
		LocalUser.Instance.SetPlayerManager(this);
		OnPlayersLoaded?.Invoke();
	}

	public void SwitchToFirstPerson()
	{
		if (!_forceFirstPersonMode) _localPlayerActions.SwitchToFirstPerson();
	}

	public void SwitchToSpectator()
	{
		if (!_forceFirstPersonMode) _localPlayerActions.SwitchToSpectator();
	}
}
