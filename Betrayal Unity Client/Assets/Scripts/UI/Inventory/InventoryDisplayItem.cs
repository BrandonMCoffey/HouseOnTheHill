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
}
