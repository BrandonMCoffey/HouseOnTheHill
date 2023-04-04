using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraSwitcher : MonoBehaviour
{
	[SerializeField, ReadOnly] private int _activeIndex = -1;
	[SerializeField, ReadOnly] private Transform _cameraTransform;
	[SerializeField, ReadOnly] private List<MainCameraOption> _enabledOptions;
	
	public void SetCamera(MainCameraOption option)
	{
		_enabledOptions.Add(option);
		_activeIndex = _enabledOptions.Count - 1;
		RefreshActive();
	}
	
	public void RemoveCamera(MainCameraOption option)
	{
		_enabledOptions.Remove(option);
		RefreshActive();
	}
	
	private void RefreshActive()
	{
		if (_activeIndex >= _enabledOptions.Count)
		{
			_activeIndex = _enabledOptions.Count - 1;
		}
		_cameraTransform = _activeIndex >= 0 ? _enabledOptions[_activeIndex].transform : null;
	}
	
	private void LateUpdate()
	{
		if (_activeIndex >= 0)
		{
			transform.SetPositionAndRotation(_cameraTransform.position, _cameraTransform.rotation);
		}
	}
}
