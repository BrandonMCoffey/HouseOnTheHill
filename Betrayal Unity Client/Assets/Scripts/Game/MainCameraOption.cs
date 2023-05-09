using UnityEngine;

public class MainCameraOption : MonoBehaviour
{
	private MainCameraSwitcher _switcher;
	
	private void Awake()
	{
		_switcher = FindObjectOfType<MainCameraSwitcher>();
	}
	
	private void OnEnable()
	{
		_switcher.SetCamera(this);
	}
	
	private void OnDisable()
	{
		_switcher.RemoveCamera(this);
	}
}
