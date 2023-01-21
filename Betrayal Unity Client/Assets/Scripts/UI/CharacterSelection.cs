using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelection : MonoBehaviour
{
	[SerializeField] private List<CharacterButton> _options;
	
	public void Select(string name)
	{
		DeselectAll();
		NetworkManager.LocalUser.SetCharacter(name);
	}
	
	private void DeselectAll()
	{
		foreach (var button in _options)
		{
			button.Deselect();
		}
	}
}
