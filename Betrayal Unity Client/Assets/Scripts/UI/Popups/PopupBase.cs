using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
	[SerializeField] private RectTransform _popup;
	[SerializeField] private Image _background;
	[SerializeField] private float _popupOpenTime = 0.5f;
	[SerializeField] private float _popupCloseTime = 0.1f;
	[SerializeField] private AnimationCurve _popupCurve
		= new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	
	private float _delta;
	private Coroutine _routine;
	private Color _backgroundColor = Color.white;
	private float _backgroundAlpha = 1;
	
	private void Awake()
	{
		if (_background)
		{
			_backgroundColor = _background.color;
			_backgroundAlpha = _backgroundColor.a;
		}
	}
	
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
		_routine = StartCoroutine(ScaleRoutine(0, CanvasController.OpenHud));
	}
	
	private IEnumerator ScaleRoutine(float goal, System.Action onComplete = null)
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
		onComplete?.Invoke();
		_routine = null;
	}
	
	private void UpdatePopup()
	{
		float s = _popupCurve.Evaluate(_delta);
		_popup.localScale = new Vector3(s, s, 1);
		if (_background)
		{
			_backgroundColor.a = _delta * _backgroundAlpha;
			_background.color = _backgroundColor;
		}
	}
}
