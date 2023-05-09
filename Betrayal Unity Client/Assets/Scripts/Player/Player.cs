using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
	[SerializeField] private GameObject _art;
	[SerializeField] private Transform _cameraParent;
	[SerializeField] private TMP_Text _userName;
	
	[Header("Debug")]
	[SerializeField, ReadOnly] private User _user;
	[SerializeField, ReadOnly] private Character _character;
	
	[SerializeField, ReadOnly] private int _speedIndex;
	[SerializeField, ReadOnly] private int _mightIndex;
	[SerializeField, ReadOnly] private int _sanityIndex;
	[SerializeField, ReadOnly] private int _knowledgeIndex;
	
	[SerializeField, ReadOnly] private List<Item> _itemsHeld;
	
	public System.Action OnItemsUpdated = delegate { };
	
	public bool IsLocal => _user.IsLocal;
	public Character Character => _character;
	public List<Item> ItemsHeld => _itemsHeld;
	
	private void OnDestroy()
	{
		PlayerManager.Players.Remove(this);
	}
	
	public void SetUser(User user)
	{
		_user = user;
		if (_userName) _userName.text = user.UserName + (IsLocal ? " (You)" : "");
	}
	
	public void SetCharacter(Character character)
	{
		_character = character;
		_speedIndex = _character.GetTraitIndex(Trait.Speed);
		_mightIndex = _character.GetTraitIndex(Trait.Might);
		_sanityIndex = _character.GetTraitIndex(Trait.Sanity);
		_knowledgeIndex = _character.GetTraitIndex(Trait.Knowledge);
		
		var art = character.Art;
		if (art)
		{
			var parent = _art.transform.parent;
			Destroy(_art);
			_art = Instantiate(art, parent);
		}
	}
	
	public int GetTraitIndex(Trait trait)
	{
		return trait switch
		{
			Trait.Speed => _speedIndex,
			Trait.Might => _mightIndex,
			Trait.Sanity => _sanityIndex,
			Trait.Knowledge => _knowledgeIndex,
			_ => -1,
		};
	}
	
	public void SetPositionAndRotationValues(Vector3 pos, Quaternion rot, Quaternion cameraRot)
	{
		transform.SetPositionAndRotation(pos, rot);
		_cameraParent.transform.localRotation = cameraRot;
	}
	
	public void CollectItem(Item item)
	{
		_itemsHeld.Add(item);
		OnItemsUpdated?.Invoke();
		CanvasController.OpenItemPrompt(item);
	}
}
