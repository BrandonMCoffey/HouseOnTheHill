using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelection : MonoBehaviour
{
	[SerializeField] private CharacterButton _baseCharacter;
	[SerializeField] private Transform _characterParent;
	
	[SerializeField, ReadOnly] private List<CharacterButton> _options;
	
	private void OnEnable()
	{
		User.OnUpdatePlayerStates += LockCharacters;
		LocalUser.Instance.OnChoosingCharacter();
	}
	
	private void OnDisable()
	{
		User.OnUpdatePlayerStates -= LockCharacters;
		LocalUser.Instance.StopChoosingCharacter();
	}
	
	private void Start()
	{
		_options = new List<CharacterButton>(GameData.CharacterCount);
		_baseCharacter.gameObject.SetActive(true);
		for (int i = 0; i < GameData.CharacterCount; i++)
		{
			var c = Instantiate(_baseCharacter, _characterParent);
			c.SetController(this);
			c.SetCharacterIndex(i);
			_options.Add(c);
		}
		_baseCharacter.gameObject.SetActive(false);
	}
	
	public void Close() => gameObject.SetActive(false);
	
	public void Select(int i)
	{
		DeselectAll();
		LocalUser.Instance.SetCharacter(i);
		Close();
	}
	
	private void DeselectAll()
	{
		foreach (var option in _options)
		{
			option.Deselect();
		}
	}
	
	private void LockCharacters()
	{
		foreach (var option in _options)
		{
			option.SetDisabled(User.RemoteCharacters.Contains(option.Index));
		}
	}
}
