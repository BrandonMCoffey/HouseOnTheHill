using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPickupDisplay : PopupBase
{
	[Header("References")]
	[SerializeField] private TMP_Text _itemNameText;
	[SerializeField] private TMP_Text _itemDescriptionText;
	[SerializeField] private Image _itemIconImage;
	[SerializeField] private TMP_Text _itemEffectsText;
	
	public void OpenPopup(Item item)
	{
		_itemNameText.text = item.Name;
		_itemDescriptionText.text = item.Description;
		_itemIconImage.sprite = item.IconSprite;
		_itemEffectsText.text = item.EffectsDescription;
		base.OpenPopup();
	}
}
