using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelection : MonoBehaviour
{
	[SerializeField] private CharacterButton _baseCharacter;
	
	[SerializeField, ReadOnly] private List<CharacterButton> _options;
	
	private void OnEnable()
	{
		User.OnUpdatePlayerStates += LockCharacters;
	}
	
	private void OnDisable()
	{
		User.OnUpdatePlayerStates -= LockCharacters;
	}
	
	private void Start()
	{
		_options = new List<CharacterButton>(GameState.CharacterCount);
		_baseCharacter.gameObject.SetActive(true);
		for (int i = 0; i < GameState.CharacterCount; i++)
		{
			var c = Instantiate(_baseCharacter, transform);
			c.SetCharacterIndex(i);
			_options.Add(c);
		}
		_baseCharacter.gameObject.SetActive(false);
	}
	
	public void Select(int i)
	{
		DeselectAll();
		LocalUser.SetCharacter(i);
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
