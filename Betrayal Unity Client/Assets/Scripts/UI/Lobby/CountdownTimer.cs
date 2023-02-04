using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
	[SerializeField] private bool _countUp = true;
	[SerializeField] private bool _round = true;
	[SerializeField] private string _beforeText;
	[SerializeField] private string _afterText;
	
	[SerializeField] private GameObject _objToHide;
	[SerializeField] private TMP_Text _countdownText;
	
	private Coroutine _routine;
	
	[Button]
	public void StartTimer(float timerLength)
	{
		SetTimerActive(true);
		_routine = StartCoroutine(TimerRoutine(timerLength));
	}
	
	[Button]
	public void StopTimer()
	{
		SetTimerActive(false);
		if (_routine != null) StopCoroutine(_routine);
		_routine = null;
	}
	
	private IEnumerator TimerRoutine(float timerLength)
	{
		for (float t = 0; t < timerLength; t += Time.deltaTime)
		{
			string timer = (_countUp ? t : timerLength - t).ToString(_round ? "F0" : "F2");
			_countdownText.text = $"{_beforeText}{timer}{_afterText}";
			yield return null;
		}
		_routine = null;
	}
	
	private void SetTimerActive(bool active)
	{
		if (_objToHide) _objToHide.SetActive(!active);
		if (_countdownText) _countdownText.gameObject.SetActive(active);
	}
}
