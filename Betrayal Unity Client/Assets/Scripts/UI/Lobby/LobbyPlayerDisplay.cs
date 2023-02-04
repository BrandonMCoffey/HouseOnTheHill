using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyPlayerDisplay : MonoBehaviour
{
	[Header("Colors")]
	[SerializeField] private Color _notReadyColor = Color.red;
	[SerializeField] private Color _readyColor = Color.green;
	
	[Header("References")]
	[SerializeField] private TMP_Text _nameText;
	[SerializeField] private Image _readyImage;
	[SerializeField] private Image _checkmarkImage;
	
	private static Color _checkmarkColor = Color.black;
	private static Color _invisibleColor = new Color(0, 0, 0, 0);
	
	public void SetName(string userName, string characterName)
	{
		_nameText.text = $"{userName} ({characterName})";
	}
	
	public void SetReady(bool ready)
	{
		_readyImage.color = ready ? _readyColor : _notReadyColor;
		_checkmarkImage.color = ready ? _checkmarkColor : _invisibleColor;
	}
}
