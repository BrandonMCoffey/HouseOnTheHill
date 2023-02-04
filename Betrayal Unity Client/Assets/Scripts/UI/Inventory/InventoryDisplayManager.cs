using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayManager : MonoBehaviour
{
    [SerializeField] private InventoryDisplayItem _baseItem;

    private Transform _parent;

    private void Start()
    {
        _parent = _baseItem.transform.parent;
        _baseItem.gameObject.SetActive(false);
        UpdateItems();
    }

    [Button(Mode = ButtonMode.InPlayMode)]
    private void UpdateItems()
    {
        
    }

    private InventoryDisplayItem CreateItem() => Instantiate(_baseItem, _parent);
}
