using System.Collections;
using CoffeyUtils;
using UnityEngine;
using TMPro;

public class EnterRoomDisplay : MonoBehaviour
{
	[SerializeField] private float _waitTime = 1;
	[SerializeField] private float _scaleTime = 1;
	[SerializeField] private RectTransform _rect;
	[SerializeField] private CanvasGroup _group;
	[SerializeField] private TMP_Text _text;
	[SerializeField] private Vector2 _scaleMinMax = new Vector2(1, 2);
	
	private void Awake()
	{
		_group.alpha = 0;
	}
	
	[Button]
	public void DisplayRoomName(string name)
	{
		_text.text = name;
		StartCoroutine(DisplayRoomNameRoutine());
	}
	
	private IEnumerator DisplayRoomNameRoutine()
	{
		yield return new WaitForSeconds(_waitTime);
		var mult = 1f / _scaleTime;
		for (float t = 0; t < 1; t += Time.deltaTime * mult)
		{
			if (t < 0.2f)
			{
				_group.alpha = t * 5;
			}
			var scale = Mathf.Lerp(_scaleMinMax.x, _scaleMinMax.y, t);
			_rect.localScale = new Vector3(scale, scale, 1);
			if (t > 0.8f)
			{
				_group.alpha = 1 - (t - 0.8f) * 5;
			}
			yield return null;
		}
		_group.alpha = 0;
	}
}
