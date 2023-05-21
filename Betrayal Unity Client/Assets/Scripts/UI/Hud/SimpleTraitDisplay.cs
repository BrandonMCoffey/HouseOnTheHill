using CoffeyUtils;
using TMPro;
using UnityEngine;

public class SimpleTraitDisplay : MonoBehaviour
{
	[SerializeField] protected TMP_Text _trait1;
	[SerializeField] protected TMP_Text _trait2;
	[SerializeField] protected TMP_Text _trait3;
	[SerializeField] protected TMP_Text _trait4;
    
	[Header("Debug")]
	[SerializeField, ReadOnly] private int _selectedTrait;
    
	private bool _refreshDisplay = true;
	
	private void OnEnable()
	{
		_refreshDisplay = true;
	}
	
	private void Update()
	{
		if (_refreshDisplay) UpdateDisplay();
	}
	
	[Button(Mode = RuntimeMode.OnlyPlaying)]
	protected void UpdateDisplay()
	{
		var player = CanvasController.LocalPlayer;
		if (player == null || player.Character == null) return;
		_trait1.text = player.GetTraitIndex(Trait.Might).ToString();
		_trait2.text = player.GetTraitIndex(Trait.Speed).ToString();
		_trait3.text = player.GetTraitIndex(Trait.Sanity).ToString();
		_trait4.text = player.GetTraitIndex(Trait.Knowledge).ToString();
		_refreshDisplay = false;
	}
	
	[Button]
	public void UpdateDisplay(Character character)
	{
		_trait1.text = character.GetDefaultTraitValue(Trait.Might).ToString();
		_trait2.text = character.GetDefaultTraitValue(Trait.Speed).ToString();
		_trait3.text = character.GetDefaultTraitValue(Trait.Sanity).ToString();
		_trait4.text = character.GetDefaultTraitValue(Trait.Knowledge).ToString();
	}
}