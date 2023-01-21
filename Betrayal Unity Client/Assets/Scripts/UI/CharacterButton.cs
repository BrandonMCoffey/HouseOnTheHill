using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterButton : MonoBehaviour
{
	[SerializeField] private string _characterName;
	[SerializeField] private Color _characterColor = Color.black;
	
	[Header("Button Colors")]
	[SerializeField] private Color _idleColor = Color.white;
	[SerializeField] private Color _selectedColor = Color.green;
	[SerializeField] private Color _disabledColor = Color.gray;
	
	[Header("References")]
	[SerializeField] private CharacterSelection _controller;
	[SerializeField] private TMP_Text _text;
	[SerializeField] private Image _image;
	[SerializeField] private Button _button;
	[SerializeField] private Image _buttonImage;
	
	[Header("Debug")]
	[SerializeField, ReadOnly] private bool _selected;
	[SerializeField, ReadOnly] private bool _disabled;
	
	private void OnValidate()
	{
		if (!_controller) _controller = transform.parent.GetComponent<CharacterSelection>();
		if (_text) _text.text = _characterName;
		if (_image) _image.color = _characterColor;
		if (_button) _buttonImage = _button.GetComponent<Image>();
	}
	
	private void Start()
	{
		_button.onClick.AddListener(Select);
	}
	
	public void Select()
	{
		if (_disabled) return;
		_controller.Select(_characterName);
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
