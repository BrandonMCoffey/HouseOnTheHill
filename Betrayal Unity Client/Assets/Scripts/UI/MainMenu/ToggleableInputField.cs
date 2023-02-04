using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToggleableInputField : MonoBehaviour
{
	[SerializeField] private string _default;
	[SerializeField, ReadOnly] private bool _useInput;
	[SerializeField] private TMP_InputField _inputField;
	[SerializeField] private GameObject _block;
	
	public string Value => _useInput && !string.IsNullOrEmpty(_inputField.text)
						? _inputField.text : _default;
	
	[Button]
	public void ToggleUseInput()
	{
		_useInput = !_useInput;
		UpdateUseInput();
	}
	
	private void UpdateUseInput()
	{
		_block.SetActive(!_useInput);
	}
}
