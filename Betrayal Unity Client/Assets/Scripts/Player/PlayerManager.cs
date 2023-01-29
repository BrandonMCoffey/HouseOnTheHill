using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private GameObject _localPlayer;
	[SerializeField] private GameObject _remotePlayer;
	
	[SerializeField, ReadOnly] private List<GameObject> _players;
	
	private void Start()
	{
		NetworkManager.OnGameLoaded();
		
		foreach (var user in User.AllUsers)
		{
			if (user.IsPlayer)
			{
				var player = Instantiate(user.IsLocal ? _localPlayer : _remotePlayer, transform);
				_players.Add(player);
			}
		}
	}
}
