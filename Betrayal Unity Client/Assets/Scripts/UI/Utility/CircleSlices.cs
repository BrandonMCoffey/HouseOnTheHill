using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSlices : MonoBehaviour
{
	[SerializeField, Range(0, 8)] private int _filled = 3;
	[SerializeField, Range(1, 8)] private int _max = 6;
	[SerializeField] private Color _filledColor = Color.green;
	[SerializeField] private Color _emptyColor = Color.gray;
	
	[Header("References")]
	[SerializeField] private List<Image> _pieces;
	[SerializeField] private Image _background;
	[SerializeField] private Color _backgroundColor = Color.black;
	[SerializeField] private GameObject _enableOnFull;
	
	private static Color _invisible = new Color(0, 0, 0, 0);
	
	private void OnValidate()
	{
		_filled = Mathf.Clamp(_filled, 0, _max);
		if (_background) _background.color = _backgroundColor;
		UpdateSlices();
	}
	
	public void SetMax(int max)
	{
		_max = Mathf.Clamp(max, 0, 8);
		UpdateSlices();
	}
	
	public void SetFilled(int filled)
	{
		_filled = filled;
		_filled = Mathf.Clamp(_filled, 0, _max);
		UpdateSlices();
	}
	
	private void UpdateSlices()
	{
		if (_pieces.Count <= 0) return;
		float rotAngle = 360f / _max;
		float fillAmount = 1f / _max - 0.005f;
		int i = 0;
		for (; i < _max; i++)
		{
			_pieces[i].transform.localEulerAngles = new Vector3(0, 0, -i * rotAngle);
			_pieces[i].fillAmount = fillAmount;
			_pieces[i].color = i < _filled ? _filledColor : _emptyColor;
		}
		for (; i < 8; i++)
		{
			_pieces[i].color = _invisible;
		}
		if (_enableOnFull) _enableOnFull.SetActive(_filled == _max);
	}
}
