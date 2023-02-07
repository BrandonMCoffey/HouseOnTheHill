using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpectatorMovement : MonoBehaviour
{
	[SerializeField] private GameObject _camera;
	[SerializeField, HighlightIfNull] private Transform _pivot;
	[SerializeField] private BoxCollider _bounds;
	[SerializeField] private float _rotationSensitivity = 0.1f;
	[SerializeField] private float _panningSensitivity = 0.1f;
	[SerializeField] private float _zoomingSensitivity = 0.01f;
	[SerializeField] private Vector2 _zoomMinMax = new Vector2(5, 50);

	[SerializeField, ReadOnly] private bool _rotating;
	[SerializeField, ReadOnly] private bool _panning;
	[SerializeField, ReadOnly] private Vector2 _mouseMovementInput;
	[SerializeField, ReadOnly] private float _zoom;

	public void SetCameraActive(bool active) => _camera.SetActive(active);
	
	private void Start()
	{
		_bounds.isTrigger = true;
		_zoom = -_camera.transform.localPosition.z;
	}

	private void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (_rotating) Rotate();
		else if (_panning) Pan();
	}

	public void SetRotate(bool rotate)
	{
		if (_panning) return;
		_rotating = rotate;
		HideMouse(rotate);
	}
	
	public void SetPan(bool pan)
	{
		if (_rotating) return;
		_panning = pan;
		HideMouse(pan);
	}

	private static void HideMouse(bool hide)
	{
		Cursor.lockState = hide ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !hide;
	}

	public void SetMouseMovement(Vector2 mouseMovement)
	{
		_mouseMovementInput = mouseMovement;
	}

	private void Rotate()
	{
		_pivot.Rotate(Vector3.up, _mouseMovementInput.x * _rotationSensitivity);
		_pivot.Rotate(Vector3.right, -_mouseMovementInput.y * _rotationSensitivity);
		var rot = _pivot.eulerAngles;
		if (rot.x > 180) rot.x -= 360;
		rot.x = Mathf.Clamp(rot.x, -75f, 75f);
		rot.z = 0;
		_pivot.rotation = Quaternion.Euler(rot);
	}

	private void Pan()
	{
		var offset = -_pivot.right * _mouseMovementInput.x + -_pivot.up * _mouseMovementInput.y;
		_pivot.position += offset * _panningSensitivity;
	}

	public void Zoom(float amount)
	{
		_zoom += -amount * _zoomingSensitivity;
		_zoom = Mathf.Clamp(_zoom, _zoomMinMax.x, _zoomMinMax.y);
		_camera.transform.localPosition = new Vector3(0, 0, -_zoom);
	}
}
