using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private bool _forceFirstPersonMode;
	[SerializeField] private Player _localPlayer;
	[SerializeField] private PlayerActionManager _localPlayerActions;
	[SerializeField] private Player _remotePlayerPrefab;

	public static List<Player> Players = new List<Player>();
	
	private void Start()
	{
		if (!NetworkManager.Instance) return;
		NetworkManager.OnGameLoaded();
		foreach (var pair in NetworkManager.AllUsers)
		{
			var user = pair.Value;
			if (user.Character > 0)
			{
				bool isLocal = user.IsLocal;
				var player = isLocal ? _localPlayer : Instantiate(_remotePlayerPrefab, transform);
				user.SetPlayer(player);
				Players.Add(player);
				if (user.IsLocal) _localPlayer = player;
			}
		}
		if (_forceFirstPersonMode) _localPlayerActions.SwitchToFirstPerson();
		LocalUser.Instance.SetPlayerManager(this);
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
