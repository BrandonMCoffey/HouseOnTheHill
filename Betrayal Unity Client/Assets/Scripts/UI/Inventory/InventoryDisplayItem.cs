using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplayItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemType;
	[SerializeField] private TMP_Text _itemDescription;
	[SerializeField] private Button _equipButton;
	[SerializeField] private Button _unequipButton;

    [Header("Debug")]
    [SerializeField] private Item _item;

    public void SetItem(Item item)
    {
        _item = item;
        UpdateDisplay();
    }

    [Button]
    public void UpdateDisplay()
    {
        _itemName.text = _item.Name;
        _itemType.text = _item.Type.ToString();
        _itemDescription.text = _item.Description;
        _itemIcon.sprite = _item.IconSprite;
    }
    
	public void EquipItem()
	{
		CanvasController.UnequipAllItems();
		if (_equipButton) _equipButton.gameObject.SetActive(false);
		if (_unequipButton) _unequipButton.gameObject.SetActive(true);
		GameController.EquipItem(_item);
		CanvasController.OpenHud();
	}
	
	public void UnequipItem(bool updatePlayer)
	{
		if (_equipButton) _equipButton.gameObject.SetActive(true);
		if (_unequipButton) _unequipButton.gameObject.SetActive(false);
		
		if (updatePlayer) GameController.EquipItem(null);
	}
}
