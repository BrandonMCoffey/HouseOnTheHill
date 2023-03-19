using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerController : MonoBehaviour
{
	[SerializeField] private LobbyPlayerDisplay _baseDisplay;
	[SerializeField] private Transform _joinButton;
	[SerializeField] private List<LobbyPlayerDisplay> _displays;
	
	private void OnEnable()
	{
		NetworkManager.OnUpdateAllUsers += UpdateCharacterList;
		User.OnUpdatePlayerStates += UpdateCharacterList;
	}
	
	private void OnDisable()
	{
		NetworkManager.OnUpdateAllUsers -= UpdateCharacterList;
		User.OnUpdatePlayerStates -= UpdateCharacterList;
	}
	
	private void Start()
	{
		_baseDisplay.gameObject.SetActive(false);
	}
	
	private void UpdateCharacterList()
	{
		_joinButton.gameObject.SetActive(true);
		int i = 0;
		foreach ((ushort id, User user) in NetworkManager.AllUsers)
		{
			TryGetCreateDisplay(i++, user);
		}
		for (; i < _displays.Count; i++)
		{
			_displays[i].gameObject.SetActive(false);
		}
	}
	
	private void TryGetCreateDisplay(int index, User user)
	{
		if (index >= _displays.Count)
		{
			_displays.Add(Instantiate(_baseDisplay, transform));
		}
		_displays[index].SetUser(user.IsLocal, user.UserName, GameData.GetCharacter(user.Character));
		_displays[index].SetReady(user.Ready);
		_displays[index].gameObject.SetActive(true);
		
		if (user.IsLocal) _joinButton.gameObject.SetActive(false);
		_joinButton.SetParent(null);
		_joinButton.SetParent(transform);
	}
}
