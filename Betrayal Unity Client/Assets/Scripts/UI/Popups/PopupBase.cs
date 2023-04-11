using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
	[SerializeField] private RectTransform _popup;
	[SerializeField] private CanvasGroup _alphaGroup;
	[SerializeField] private float _popupOpenTime = 0.5f;
	[SerializeField] private float _popupCloseTime = 0.1f;
	[SerializeField] private AnimationCurve _popupCurve
		= new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
		
	public float OpenTime => _popupOpenTime;
	public float CloseTime => _popupCloseTime;
	
	private float _delta;
	private Coroutine _routine;
	
	[Button(Mode = ButtonMode.InPlayMode, Spacing = 20)]
	public virtual void OpenPopup()
	{
		if (_routine != null) StopCoroutine(_routine);
		_routine = StartCoroutine(ScaleRoutine(1));
	}
	
	[Button(Mode = ButtonMode.InPlayMode)]
	public virtual void ClosePopup()
	{
		if (_routine != null) StopCoroutine(_routine);
		_routine = StartCoroutine(ScaleRoutine(0));
	}
	
	private IEnumerator ScaleRoutine(float goal)
	{
		if (_delta == goal) yield return null;
		else if (_delta < goal)
		{
			float openMult = 1f / _popupOpenTime;
			while (_delta < goal)
			{
				_delta += Time.deltaTime * openMult;
				UpdatePopup();
				yield return null;
			}
			_delta = 1;
			UpdatePopup();
		}
		else
		{
			float closeMult = 1f / _popupCloseTime;
			while (_delta > goal)
			{
				_delta -= Time.deltaTime * closeMult;
				UpdatePopup();
				yield return null;
			}
			_delta = 0;
			UpdatePopup();
		}
		_routine = null;
	}
	
	private void UpdatePopup()
	{
		if (_popup)
		{
			float s = _popupCurve.Evaluate(_delta);
			_popup.localScale = new Vector3(s, s, 1);
		}
		if (_alphaGroup)
		{
			_alphaGroup.blocksRaycasts = _delta > 0;
			_alphaGroup.interactable = _delta == 1;
			_alphaGroup.alpha = _delta;
		}
	}
}
