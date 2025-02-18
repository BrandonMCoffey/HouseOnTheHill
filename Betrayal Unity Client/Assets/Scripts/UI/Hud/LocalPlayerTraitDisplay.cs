﻿using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;
using TMPro;

public class LocalPlayerTraitDisplay : MonoBehaviour
{
	[SerializeField] private TMP_Text _characterName;
	[SerializeField] private List<FullTraitDisplay> _traitDisplays = new List<FullTraitDisplay>();
	
	private bool _refreshDisplay = true;
	
	private void OnEnable()
	{
		_refreshDisplay = true;
	}
	
	private void Update()
	{
		if (_refreshDisplay) UpdateDisplays();
	}
	
	[Button(Mode = RuntimeMode.OnlyPlaying)]
	private void UpdateDisplays()
	{
		var player = CanvasController.LocalPlayer;
		if (player == null || player.Character == null) return;
		_characterName.text = player.Character.Name;
		foreach (var display in _traitDisplays)
		{
			display.SetPlayer(player);
			display.UpdateDisplay();
		}
		_refreshDisplay = false;
	}
}
