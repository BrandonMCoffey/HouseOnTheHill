using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
	public static EventController Instance;
	
	[SerializeField] private List<EventBase> _events;
	[SerializeField] private List<Item> _items;

	[SerializeField] private float _eventPopupDelay = 2;
	[SerializeField] private CollectableItem _itemPrefab;
	[SerializeField] private Vector3 _collectableRoomOffset = Vector3.up;
	
	public static System.Action<int> OnUpdateItemsToCollect = delegate { };
	
	private List<Item> _itemsToCollect = new List<Item>();
	
	private void Awake()
	{
		Instance = this;
	}
	
	public void CreateEvent(Room room)
	{
		StartCoroutine(CreateEventRoutine(room));
	}

	private IEnumerator CreateEventRoutine(Room room)
	{
		yield return new WaitForSeconds(_eventPopupDelay);
		var e = GetRandomEvent();
		CanvasController.OpenEventPrompt(e.Name, e.Description);
	}
	
	public void CreateOmen(Room room)
	{
		var collectable = CreateCollectableItem(room);
		collectable.SetItem(GetRandomOmen());
	}
	
	public void CreateIem(Room room)
	{
		var collectable = CreateCollectableItem(room);
		var item = GetRandomItem();
		collectable.SetItem(item);
		_itemsToCollect.Add(item);
		OnUpdateItemsToCollect?.Invoke(_itemsToCollect.Count);
	}
	
	public void CreateTwoItems(Room room)
	{
		var collectable1 = CreateCollectableItem(room);
		collectable1.transform.position -= Vector3.right;
		var item1 = GetRandomItem();
		collectable1.SetItem(item1);
		_itemsToCollect.Add(item1);
		var collectable2 = CreateCollectableItem(room);
		collectable2.transform.position += Vector3.right;
		var item2 = GetRandomItem();
		collectable2.SetItem(item2);
		_itemsToCollect.Add(item2);
		OnUpdateItemsToCollect?.Invoke(_itemsToCollect.Count);
	}
	
	private CollectableItem CreateCollectableItem(Room room)
	{
		var pos = room.transform.position + _collectableRoomOffset;
		return Instantiate(_itemPrefab, pos, Quaternion.identity, transform);
	}
	
	public void CollectItem(Item item)
	{
		_itemsToCollect.Remove(item);
		CanvasController.LocalPlayer.CollectItem(item);
		OnUpdateItemsToCollect?.Invoke(_itemsToCollect.Count);
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
