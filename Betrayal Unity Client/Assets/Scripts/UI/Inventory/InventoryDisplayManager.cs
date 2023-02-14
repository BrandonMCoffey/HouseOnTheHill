using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayManager : MonoBehaviour
{
	[SerializeField] private InventoryDisplayItem _baseItem;
	[SerializeField, ReadOnly] private List<InventoryDisplayItem> _items;

	[SerializeField, ReadOnly] private Transform _parent;
	[SerializeField, ReadOnly] private bool _refreshDisplay;
	[SerializeField, ReadOnly] private bool _connectPlayer;

    private void Start()
    {
        _parent = _baseItem.transform.parent;
	    _baseItem.gameObject.SetActive(false);
	    _refreshDisplay = true;
	    _connectPlayer = true;
    }
	
	private void Update()
	{
		if (_refreshDisplay) UpdateItems();
		if (_connectPlayer && CanvasController.LocalPlayer)
		{
			CanvasController.LocalPlayer.OnItemsUpdated += RefreshDisplay;
		}
	}
	
	[Button(Mode = ButtonMode.InPlayMode)]
	private void RefreshDisplay() => _refreshDisplay = true;
	
    private void UpdateItems()
    {
	    var player = CanvasController.LocalPlayer;
	    if (player == null) return;
	    int items = player.ItemsHeld.Count;
	    for (int i = _items.Count; i < items; i++)
	    {
	    	_items.Add(CreateItem());
	    }
	    for (int i = 0; i < items; i++)
	    {
	    	_items[i].gameObject.SetActive(true);
	    	_items[i].SetItem(player.ItemsHeld[i]);
	    }
	    for (int i = items; i < _items.Count; i++)
	    {
	    	_items[i].gameObject.SetActive(false);
	    }
	    _refreshDisplay = false;
    }

    private InventoryDisplayItem CreateItem() => Instantiate(_baseItem, _parent);
}
