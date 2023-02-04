using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterButton : MonoBehaviour
{
	[SerializeField, ReadOnly] private int _characterIndex;
	
	[Header("Button Colors")]
	[SerializeField] private Color _idleColor = Color.white;
	[SerializeField] private Color _selectedColor = Color.green;
	[SerializeField] private Color _disabledColor = Color.gray;
	
	[Header("References")]
	[SerializeField] private CharacterSelection _controller;
	[SerializeField] private TMP_Text _nameText;
	[SerializeField] private Image _colorImage;
	[SerializeField] private Button _button;
	[SerializeField] private Image _buttonImage;
	
	[Header("Debug")]
	[SerializeField, ReadOnly] private bool _selected;
	[SerializeField, ReadOnly] private bool _disabled;
	
	public int Index => _characterIndex;
	
	private void OnValidate()
	{
		if (!_controller) _controller = transform.parent.GetComponent<CharacterSelection>();
		if (_button) _buttonImage = _button.GetComponent<Image>();
	}
	
	private void OnEnable()
	{
		_button.onClick.AddListener(Select);
	}
	
	private void OnDisable()
	{
		_button.onClick.RemoveListener(Select);
	}
	
	public void SetCharacterIndex(int i)
	{
		_characterIndex = i;
		SetupCharacter();
	}
	
	[Button]
	private void SetupCharacter()
	{
		var character = GameState.GetCharacter(_characterIndex);
		_nameText.text = character.Name;
		_colorImage.color = character.Color;
	}
	
	public void Select()
	{
		if (_disabled) return;
		_controller.Select(_characterIndex);
		_selected = true;
		_button.interactable = false;
		_buttonImage.color = _selectedColor;
	}
	
	public void Deselect()
	{
		if (_disabled) return;
		_selected = false;
		_button.interactable = true;
		_buttonImage.color = _idleColor;
	}
	
	public void SetDisabled(bool disabled = true)
	{
		if (_selected) return;
		_disabled = disabled;
		_button.interactable = !disabled;
		_buttonImage.color = disabled ? _disabledColor : _idleColor;
	}
}
