using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleTraitDisplay : MonoBehaviour
{
	[SerializeField] private Color _highlightedTraitTextColor = Color.black;
	[SerializeField] private Color _traitTextColor = Color.Lerp(Color.black, Color.yellow, 0.4f);
	[SerializeField] private bool _highlightTrait1;
	[SerializeField] private bool _highlightTrait2;
	[SerializeField] private bool _highlightTrait3;
	[SerializeField] private bool _highlightTrait4;
	
	[Header("References")]
    [SerializeField] private TMP_Text _trait1;
    [SerializeField] private TMP_Text _trait2;
    [SerializeField] private TMP_Text _trait3;
    [SerializeField] private TMP_Text _trait4;
    [SerializeField] private GameObject _trait1Highlight;
    [SerializeField] private GameObject _trait2Highlight;
    [SerializeField] private GameObject _trait3Highlight;
	[SerializeField] private GameObject _trait4Highlight;
    
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
	
    [Button(Mode = ButtonMode.InPlayMode)]
    private void UpdateDisplay()
    {
        var player = CanvasController.LocalPlayer;
        if (player == null || player.Character == null) return;
        _trait1.text = player.GetTraitIndex(Trait.Might).ToString();
        _trait2.text = player.GetTraitIndex(Trait.Speed).ToString();
        _trait3.text = player.GetTraitIndex(Trait.Sanity).ToString();
        _trait4.text = player.GetTraitIndex(Trait.Knowledge).ToString();
        _refreshDisplay = false;
    }
    
	public void HighlightTrait(int trait, bool highlight = true)
	{
		var c = highlight ? _highlightedTraitTextColor : _traitTextColor;
		switch (trait)
		{
		case 1:
			_trait1.color = c;
			_trait1Highlight.SetActive(highlight);
			break;
		case 2:
			_trait2.color = c;
			_trait2Highlight.SetActive(highlight);
			break;
		case 3:
			_trait3.color = c;
			_trait3Highlight.SetActive(highlight);
			break;
		case 4:
			_trait4.color = c;
			_trait4Highlight.SetActive(highlight);
			break;
		}
	}
	
	[Button]
	public void ResetTraitHighlights()
	{
		HighlightTrait(1, _highlightTrait1);
		HighlightTrait(2, _highlightTrait2);
		HighlightTrait(3, _highlightTrait3);
		HighlightTrait(4, _highlightTrait4);
	}
}
