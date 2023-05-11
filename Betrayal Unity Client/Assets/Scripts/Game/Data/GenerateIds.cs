using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Betrayal/Utility/GenerateIds")]
public class GenerateIds : ScriptableObject
{
	[SerializeField] private List<Item> _items;
	[SerializeField] private List<EventBase> _events;
	[SerializeField] private List<Room> _rooms;
	
	[Button]
	private void GenerateItemIds()
	{
		int id = 0;
		foreach (var i in _items)
		{
			i.SetId(id++);
		}
	}
	
	[Button]
	private void GenerateEventIds()
	{
		int id = 0;
		foreach (var e in _events)
		{
			e.SetId(id++);
		}
	}
	
	[Button]
	private void GenerateRoomIds()
	{
		int id = 0;
		foreach (var r in _rooms)
		{
			r.SetId(id++);
		}
	}
}
