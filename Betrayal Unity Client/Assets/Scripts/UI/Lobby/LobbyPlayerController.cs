using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LobbyPlayerController : MonoBehaviour
{
	[SerializeField] private LobbyPlayerDisplay _baseDisplay;
	[SerializeField] private Transform _displayParent;
	[SerializeField] private Transform _joinButton;
	[SerializeField] private List<LobbyPlayerDisplay> _displays;
	
	[Header("Spectators")]
	[SerializeField] private TMP_Text _baseSpectator;
	[SerializeField] private Transform _spectatorsParent;
	[SerializeField] private List<TMP_Text> _spectators;

	[Header("Character Selection")]
	[SerializeField] private Canvas _characterSelection;
	
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
		_baseSpectator.gameObject.SetActive(false);
	}
	
	private void UpdateCharacterList()
	{
		_joinButton.gameObject.SetActive(true);
		int s = 0;
		int d = 0;
		foreach ((ushort id, User user) in NetworkManager.AllUsers)
		{
			if (user.Character == -1) TryGetCreateSpectator(s++, user.UserName);
			else if (user.Character > -10) TryGetCreateDisplay(d++, user);
		}
		for (; s < _spectators.Count; s++)
		{
			_spectators[s].gameObject.SetActive(false);
		}
		for (; d < _displays.Count; d++)
		{
			_displays[d].gameObject.SetActive(false);
		}
	}
	
	private void TryGetCreateSpectator(int index, string userName)
	{
		if (index >= _spectators.Count)
		{
			_spectators.Add(Instantiate(_baseSpectator, _spectatorsParent));
		}
		_spectators[index].text = userName;
		_spectators[index].gameObject.SetActive(true);
	}
	
	private void TryGetCreateDisplay(int index, User user)
	{
		if (index >= _displays.Count)
		{
			_displays.Add(Instantiate(_baseDisplay, _displayParent));
		}
		_displays[index].SetUser(user.IsLocal, user.UserName, GameData.GetCharacter(user.Character));
		_displays[index].SetReady(user.Ready);
		_displays[index].gameObject.SetActive(true);
		
		if (user.IsLocal) _joinButton.gameObject.SetActive(false);
		_joinButton.SetParent(null);
		_joinButton.SetParent(_displayParent);
	}
	
	public void OpenCharacterSelection()
	{
		_characterSelection.enabled = true;
		LocalUser.Instance.OnChoosingCharacter();
	}
	
	public void SelectCharacter(int index)
	{
		LocalUser.Instance.SetCharacter(index);
		CloseCharacterSelection();
	}
	
	public void CloseCharacterSelection()
	{
		_characterSelection.enabled = false;
		LocalUser.Instance.StopChoosingCharacter();
	}
}
