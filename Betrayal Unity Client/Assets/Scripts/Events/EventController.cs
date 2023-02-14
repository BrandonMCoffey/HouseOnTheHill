using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
	public static EventController Instance;
	
	[SerializeField] private List<EventBase> _events;
	[SerializeField] private List<Item> _items;
	
	[SerializeField] private CollectableItem _itemPrefab;
	[SerializeField] private Vector3 _collectableRoomOffset = Vector3.up;
	
	private void Awake()
	{
		Instance = this;
	}
	
	public void CreateEvent(Room room)
	{
		
	}
	
	public void CreateOmen(Room room)
	{
		var collectable = CreateCollectableItem(room);
		collectable.SetItem(GetRandomOmen());
	}
	
	public void CreateIem(Room room)
	{
		var collectable = CreateCollectableItem(room);
		collectable.SetItem(GetRandomItem());
	}
	
	public void CreateTwoItems(Room room)
	{
		var collectable1 = CreateCollectableItem(room);
		collectable1.transform.position -= Vector3.right;
		collectable1.SetItem(GetRandomItem());
		var collectable2 = CreateCollectableItem(room);
		collectable2.transform.position += Vector3.right;
		collectable2.SetItem(GetRandomItem());
	}
	
	private CollectableItem CreateCollectableItem(Room room)
	{
		var pos = room.transform.position + _collectableRoomOffset;
		return Instantiate(_itemPrefab, pos, Quaternion.identity, transform);
	}
	
	private EventBase GetRandomEvent()
	{
		int index = Random.Range(0, _events.Count);
		var e = _events[index];
		_events.RemoveAt(index);
		return e;
	}
	private Item GetRandomItem() => GetRandomOmen(false);
	private Item GetRandomOmen(bool omen = true)
	{
		int iter = 0;
		while (true)
		{
			int index = Random.Range(0, _items.Count);
			var item = _items[index];
			_items.RemoveAt(index);
			if (item.Omen == omen) return item;
			_items.Add(item);
			if (iter++ > 1000) break;
		}
		Debug.LogError("No Omens Left in Stack");
		return null;
	}
}
