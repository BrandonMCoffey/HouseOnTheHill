using CoffeyUtils;
using TMPro;
using UnityEngine;

public class HighlightableTraitDisplay : SimpleTraitDisplay
{
	[Header("Settings")]
	[SerializeField] private Color _highlightedTraitTextColor = Color.black;
	[SerializeField] private Color _traitTextColor = Color.Lerp(Color.black, Color.yellow, 0.4f);
	[SerializeField] private bool _highlightTrait1;
	[SerializeField] private bool _highlightTrait2;
	[SerializeField] private bool _highlightTrait3;
	[SerializeField] private bool _highlightTrait4;
	
	[Header("References")]
    [SerializeField] private GameObject _trait1Highlight;
    [SerializeField] private GameObject _trait2Highlight;
    [SerializeField] private GameObject _trait3Highlight;
	[SerializeField] private GameObject _trait4Highlight;
	
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
