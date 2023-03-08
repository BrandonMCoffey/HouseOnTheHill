using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FullTraitDisplay : MonoBehaviour
{
	[SerializeField] private Player _player;
	[SerializeField] private Trait _trait;
	[SerializeField] private Color _selected = Color.white;
	
	[Header("References")]
	[SerializeField] private TMP_Text _value1;
	[SerializeField] private TMP_Text _value2;
	[SerializeField] private TMP_Text _value3;
	[SerializeField] private TMP_Text _value4;
	[SerializeField] private TMP_Text _value5;
	[SerializeField] private TMP_Text _value6;
	[SerializeField] private TMP_Text _value7;
	[SerializeField] private TMP_Text _value8;
	[SerializeField] private List<Image> _selectedValues = new List<Image>(8);
	
	private static Color _invisible = new Color(0, 0, 0, 0);
	
	public void SetPlayer(Player player) => _player = player;
	
	[Button]
	public void UpdateDisplay()
	{
		var trait = _player.Character.GetTrait(_trait);
		
		_value1.text = trait.GetValue(1).ToString();
		_value2.text = trait.GetValue(2).ToString();
		_value3.text = trait.GetValue(3).ToString();
		_value4.text = trait.GetValue(4).ToString();
		_value5.text = trait.GetValue(5).ToString();
		_value6.text = trait.GetValue(6).ToString();
		_value7.text = trait.GetValue(7).ToString();
		_value8.text = trait.GetValue(8).ToString();
		
		var index = _player.GetTraitIndex(_trait);
		for (int i = 0; i < _selectedValues.Count; i++)
		{
			_selectedValues[i].color = i == index ? _selected : _invisible;
		}
	}
}
