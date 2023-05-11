using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoffeyUtils;

public class CharacterSelectionPair : MonoBehaviour
{
	[SerializeField] private Character _character1;
	[SerializeField, ReadOnly] private int _character1Index = -9999;
	[SerializeField] private Character _character2;
	[SerializeField, ReadOnly] private int _character2Index = -9999;
	
	[Header("References")]
	[SerializeField] private LobbyPlayerController _controller;
	[SerializeField] private TMP_Text _c1Name;
	[SerializeField] private TMP_Text _c2Name;
	[SerializeField] private Button _c1Button;
	[SerializeField] private Button _c2Button;
	[SerializeField] private Image _c1Image;
	[SerializeField] private Image _c2Image;
	
	private void OnValidate()
	{
		UpdateDisplay();
	}
	
	private void Awake()
	{
		UpdateDisplay();
	}
	
	private void OnEnable()
	{
		CheckLockCharacterPair();
		User.OnUpdatePlayerStates += CheckLockCharacterPair;
	}
	
	private void OnDisable()
	{
		User.OnUpdatePlayerStates -= CheckLockCharacterPair;
	}
	
	[Button]
	private void UpdateDisplay()
	{
		if (_character1)
		{
			_character1Index = GameData.GetCharacterIndex(_character1);
			if (_c1Name) _c1Name.text = _character1.Name;
			if (_c1Image) _c1Image.color = _character1.Color;
		}
		if (_character2)
		{
			_character2Index = GameData.GetCharacterIndex(_character2);
			if (_c2Name) _c2Name.text = _character2.Name;
			if (_c2Image) _c2Image.color = _character2.Color;
		}
	}
	
	public void SelectCharacter1() => SelectCharacter(_character1Index);
	public void SelectCharacter2() => SelectCharacter(_character2Index);
	
	private void SelectCharacter(int characterIndex)
	{
		if (characterIndex < 0) return;
		_controller.SelectCharacter(characterIndex);
	}
	
	private void CheckLockCharacterPair()
	{
		bool locked = User.RemoteCharacters.Contains(_character1Index) || User.RemoteCharacters.Contains(_character2Index);
		_c1Button.interactable = !locked;
		_c2Button.interactable = !locked;
	}
}
