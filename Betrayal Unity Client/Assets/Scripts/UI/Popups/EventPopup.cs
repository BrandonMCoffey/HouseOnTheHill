using UnityEngine;
using TMPro;

public class EventPopup : PopupBase
{
	[Header("References")]
	[SerializeField] private TMP_Text _eventPopupHeader;
	[SerializeField] private TMP_Text _eventPopupDescription;
	
	public void SetValues(string header, string description)
	{
		_eventPopupHeader.text = header;
		_eventPopupDescription.text = description;
	}

	public override void ClosePopup()
	{
		base.ClosePopup();
		GameController.Instance.CheckCanEndTurn();
	}
}
