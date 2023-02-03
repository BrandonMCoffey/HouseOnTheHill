using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField, ReadOnly] private User _user;
	
	[SerializeField, ReadOnly] private Character _character;
	
	[SerializeField, ReadOnly] private int _speedIndex;
	[SerializeField, ReadOnly] private int _mightIndex;
	[SerializeField, ReadOnly] private int _sanityIndex;
	[SerializeField, ReadOnly] private int _knowledgeIndex;
	
	[SerializeField, ReadOnly] private List<int> _itemIdsHeld;
	
	public Character Character => _character;
	
	private void OnDestroy()
	{
		PlayerManager.Players.Remove(this);
	}
	
	public void SetCharacter(Character character)
	{
		_character = character;
		_speedIndex = _character.Speed.Index;
		_mightIndex = _character.Might.Index;
		_sanityIndex = _character.Sanity.Index;
		_knowledgeIndex = _character.Knowledge.Index;
	}
}
