using CoffeyUtils;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
	[SerializeField] private LayerMask _localPlayerMask;
	[SerializeField] private Item _item;
	
	public void SetItem(Item item)
	{
		_item = item;
		Instantiate(item.Prefab, transform, false);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (_localPlayerMask == (_localPlayerMask | (1 << other.gameObject.layer)))
		{
			CollectItem();
		}
	}

	[Button]
	public void CollectItem()
	{
		EventController.Instance.CollectItem(_item);
		Destroy(gameObject);
	}
}
