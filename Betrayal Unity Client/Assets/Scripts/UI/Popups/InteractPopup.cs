using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractPopup : PopupBase
{
	[Header("References")]
	[SerializeField] private TMP_Text _popupText;
	
	public void SetValues(string text)
	{
		_popupText.text = text;
	}
}
