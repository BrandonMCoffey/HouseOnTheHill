using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInventoryIconDisplay : MonoBehaviour
{
	[SerializeField] private Transform _parent;
	[SerializeField] private Image _baseImage;
	[SerializeField] private List<Image> _images = new List<Image>();
	
	private bool _refreshDisplay;
	
	private void OnEnable()
	{
		_refreshDisplay = true;
		_baseImage.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (_refreshDisplay) RefreshDisplay();
	}

	[Button(Mode = ButtonMode.InPlayMode)]
	private void RefreshDisplay()
	{
		var player = CanvasController.LocalPlayer;
		if (player == null) return;
		int i = 0;
		foreach (var item in player.ItemsHeld)
		{
			TryGetCreateDisplay(i++, item);
		}
		for (; i < _images.Count; i++)
		{
			_images[i].gameObject.SetActive(false);
		}
		_refreshDisplay = false;
	}
	
	private void TryGetCreateDisplay(int index, Item item)
	{
		if (index >= _images.Count)
		{
			_images.Add(Instantiate(_baseImage, _parent));
		}
		_images[index].gameObject.SetActive(true);
		_images[index].sprite = item.IconSprite;
	}
}
