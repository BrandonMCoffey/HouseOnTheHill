using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicTimedEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent _event = new UnityEvent();
	
	[Header("Settings")]
	[SerializeField] private bool _autoActivate = true;
	[SerializeField] private float _delay = 1;
	
	private void Start()
    {
	    if (_autoActivate) TriggerTimedEvent();
    }
    
	[Button]
	public void TriggerTimedEvent() => StartCoroutine(TriggerTimedEventRoutine());
	private IEnumerator TriggerTimedEventRoutine()
	{
		yield return new WaitForSeconds(_delay);
		TriggerEvent();
	}
    
	[Button]
	public void TriggerEvent()
	{
		_event?.Invoke();
	}
}
