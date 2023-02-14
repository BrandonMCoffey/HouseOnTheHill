using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayManager : MonoBehaviour
{
	[SerializeField] private InventoryDisplayItem _baseItem;
	[SerializeField, ReadOnly] private List<InventoryDisplayItem> _items;

	private Transform _parent;
	private bool _refreshDisplay = true;

    private void Start()
    {
        _parent = _baseItem.transform.parent;
	    _baseItem.gameObject.SetActive(false);
	    _refreshDisplay = true;
    }
	
	private void Update()
	{
		if (_refreshDisplay) UpdateItems();
	}
	

    [Button(Mode = ButtonMode.InPlayMode)]
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
