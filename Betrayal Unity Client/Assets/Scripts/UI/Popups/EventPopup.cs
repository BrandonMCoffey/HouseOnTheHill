using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventPopup : MonoBehaviour
{
	[SerializeField] private TMP_Text _eventPopupHeader;
	[SerializeField] private TMP_Text _eventPopupDescription;
	[SerializeField] private RectTransform _popup;
	[SerializeField] private float _popupOpenTime = 0.5f;
	[SerializeField] private float _popupCloseTime = 0.1f;
	[SerializeField] private AnimationCurve _popupCurve
		= new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	
	private Coroutine _routine;
	
	public void OpenPopup(string header, string description)
	{
		_eventPopupHeader.text = header;
		_eventPopupDescription.text = description;
		if (_routine != null) StopCoroutine(_routine);
		_routine = StartCoroutine(ScaleRoutine(false));
	}
	
	public void ClosePopup()
	{
		if (_routine != null) StopCoroutine(_routine);
		_routine = StartCoroutine(ScaleRoutine(true, CanvasController.OpenHud));
	}
	
	private IEnumerator ScaleRoutine(bool reverse, System.Action onComplete = null)
	{
		float mult = 1f / (reverse ? _popupCloseTime : _popupOpenTime);
		for (float t = 0; t < 1; t += Time.deltaTime * mult)
		{
			float delta = _popupCurve.Evaluate(reverse ? 1 - t : t);
			_popup.localScale = new Vector3(delta, delta, 1);
			yield return null;
		}
		_popup.localScale = new Vector3(reverse ? 0 : 1, reverse ? 0 : 1, 1);
		onComplete?.Invoke();
		_routine = null;
	}
}
