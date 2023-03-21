using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyPlayerDisplay : MonoBehaviour
{
	[SerializeField] private bool _selected;
	
	[Header("Colors")]
	[SerializeField] private Color _notReadyColor = Color.red;
	[SerializeField] private Color _readyColor = Color.green;
	[SerializeField] private Color _noCharacterPortrait = Color.gray;
	
	[Header("References")]
	[SerializeField] private TMP_Text _userNameText;
	[SerializeField] private GameObject _selectedImage;
	[SerializeField] private TMP_Text _characterNameText;
	[SerializeField] private Image _characterPortrait;
	[SerializeField] private GameObject _noCharacterSelected;
	[SerializeField] private TMP_Text _characterTrait1Text;
	[SerializeField] private TMP_Text _characterTrait2Text;
	[SerializeField] private TMP_Text _characterTrait3Text;
	[SerializeField] private TMP_Text _characterTrait4Text;
	[SerializeField] private Button _readyButton;
	[SerializeField] private Image _readyImage;
	[SerializeField] private TMP_Text _readyText;

	public void SetUser(bool isLocal, string userName, Character character)
	{
		_selected = isLocal;
		_userNameText.text = userName;
		_readyButton.interactable = isLocal;
		_selectedImage.SetActive(_selected);
		if (character)
		{
			_characterNameText.text = character.Name;
			_characterPortrait.color = character.Color;
			SetCharacterObjectsActive(true);
			_characterTrait1Text.text = character.GetDefaultTraitValue(Trait.Might).ToString();
			_characterTrait2Text.text = character.GetDefaultTraitValue(Trait.Speed).ToString();
			_characterTrait3Text.text = character.GetDefaultTraitValue(Trait.Sanity).ToString();
			_characterTrait4Text.text = character.GetDefaultTraitValue(Trait.Knowledge).ToString();
		}
		else
		{
			_characterNameText.text = "";
			_characterPortrait.color = _noCharacterPortrait;
			SetCharacterObjectsActive(false);
		}
	}
	
	private void SetCharacterObjectsActive(bool active)
	{
		_noCharacterSelected.SetActive(!active);
		_characterTrait1Text.gameObject.SetActive(active);
		_characterTrait2Text.gameObject.SetActive(active);
		_characterTrait3Text.gameObject.SetActive(active);
		_characterTrait4Text.gameObject.SetActive(active);
	}
	
	public void ToggleReady() => LocalUser.Instance.ToggleReady();
	public void SetReady(bool ready)
	{
		_readyImage.color = ready ? _readyColor : _notReadyColor;
		if (_selected) _readyText.text = ready ? "Ready!" : "Ready?";
		else _readyText.text = ready ? "Ready" : "Not Ready";
	}
}
