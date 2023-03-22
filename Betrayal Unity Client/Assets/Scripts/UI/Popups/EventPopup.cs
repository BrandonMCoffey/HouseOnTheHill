using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventPopup : PopupBase
{
	[Header("References")]
	[SerializeField] private TMP_Text _eventPopupHeader;
	[SerializeField] private TMP_Text _eventPopupDescription;
	
	public void OpenPopup(string header, string description)
	{
		_eventPopupHeader.text = header;
		_eventPopupDescription.text = description;
		base.OpenPopup();
	}
}
