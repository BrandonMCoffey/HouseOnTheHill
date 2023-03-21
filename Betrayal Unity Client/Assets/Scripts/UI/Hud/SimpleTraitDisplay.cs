using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleTraitDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _trait1;
    [SerializeField] private TMP_Text _trait2;
    [SerializeField] private TMP_Text _trait3;
    [SerializeField] private TMP_Text _trait4;
    [SerializeField] private GameObject _trait1Highlight;
    [SerializeField] private GameObject _trait2Highlight;
    [SerializeField] private GameObject _trait3Highlight;
    [SerializeField] private GameObject _trait4Highlight;
    
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
}
