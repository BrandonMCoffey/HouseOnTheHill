using CoffeyUtils;
using UnityEngine;
using TMPro;

public class ToggleableInputField : MonoBehaviour
{
	[SerializeField] private string _default;
	[SerializeField, ReadOnly] private bool _useInput;
	[SerializeField] private TMP_InputField _inputField;
	[SerializeField] private GameObject _blockInput;
	[SerializeField] private GameObject _useInputActive;
	
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
		if (_blockInput) _blockInput.SetActive(!_useInput);
		if (_useInputActive) _useInputActive.SetActive(_useInput);
	}
}
