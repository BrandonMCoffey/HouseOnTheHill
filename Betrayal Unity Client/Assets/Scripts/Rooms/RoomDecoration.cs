using System.Collections.Generic;
using System.Linq;
using CoffeyUtils;
using UnityEngine;

public class RoomDecoration : MonoBehaviour
{
	[SerializeField] private List<Light> _mainLights;
	[Header("Intensity")]
	[SerializeField] private bool _overrideIntensity;
	[SerializeField, ShowIf("_overrideIntensity")] private float _basicLightIntensity = 3;
	[SerializeField, ShowIf("_overrideIntensity")] private float _omenLightIntensity = 2;
	[Header("Color")]
	[SerializeField] private bool _overrideColor;
	[SerializeField, ShowIf("_overrideColor")] private Color _basicLightColor = Color.white;
	[SerializeField, ShowIf("_overrideColor")] private Color _omenLightColor = Color.green;
	[Header("Range")]
	[SerializeField] private bool _overrideRange;
	[SerializeField, ShowIf("_overrideRange")] private float _range = 10;

	public void UpdateLights(Room room) => UpdateLights(room.Omen);
	[Button]
	private void UpdateLights(bool omen)
	{
		foreach (Light l in _mainLights.Where(l => l != null))
		{
			if (_overrideIntensity) l.intensity = omen ? _omenLightIntensity : _basicLightIntensity;
			if (_overrideColor) l.color = omen ? _omenLightColor : _basicLightColor;
			if (_overrideRange) l.range = _range;
		}
	}
}
