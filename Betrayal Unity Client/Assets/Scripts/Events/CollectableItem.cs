using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
	[SerializeField, ReadOnly] private Item _item;
	
	public void SetItem(Item item)
	{
		_item = item;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		var player = other.GetComponent<Player>();
		if (player) CollectItem(player);
	}

	public void CollectItem(Player player)
	{
		player.CollectItem(_item);
		Destroy(gameObject);
	}
}
