using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentPlayerSplash : PopupBase
{
	[Header("Settings")]
	[SerializeField] private float _holdDuration = 1;
	[SerializeField] private string _prependToUserName = "";
	[SerializeField] private string _appendToUserName = "'s Turn";
	[SerializeField] private string _prependToCharacterName = "- ";
	[SerializeField] private string _appendToCharacterName = " -";
	
	[Header("References")]
	[SerializeField] private Canvas _canvas;
	[SerializeField] private Image _background;
	[SerializeField] private TMP_Text _usersTurnText;
	[SerializeField] private TMP_Text _characterNameText;
	
	private Coroutine _routine;
	
	private void OnEnable()
	{
		NetworkManager.OnSetUsersTurn += SetUsersTurnResponse;
	}
	
	private void OnDisable()
	{
		NetworkManager.OnSetUsersTurn -= SetUsersTurnResponse;
	}
	
	private void SetUsersTurnResponse(User user)
	{
		if (_routine != null) StopCoroutine(_routine);
		StartCoroutine(SplashRoutine(user));
	}
	
	private IEnumerator SplashRoutine(User user)
	{
		_usersTurnText.text = _prependToUserName + user.UserName + _appendToUserName;
		var character = GameData.GetCharacter(user.Character);
		_characterNameText.text = _prependToCharacterName + character.Name + _appendToCharacterName;
		var c = _background.color;
		var a = c.a;
		c = Color.Lerp(Color.white, character.Color, 0.2f);
		c.a = a;
		_background.color = c;
		_characterNameText.color = Color.Lerp(Color.black, character.Color, 0.5f);
		
		_canvas.enabled = true;
		OpenPopup();
		yield return new WaitForSeconds(OpenTime + _holdDuration);
		ClosePopup();
		yield return new WaitForSeconds(CloseTime);
		_canvas.enabled = false;
		_routine = null;
	}
}
