using UnityEngine;
using UnityEngine.EventSystems;

public class SpectatorMovement : MonoBehaviour
{
	[Header("Sensitivity")]
	[SerializeField] private float _rotationSensitivity = 0.1f;
	[SerializeField] private float _panningSensitivity = 0.1f;
	[SerializeField] private float _zoomingSensitivity = 0.01f;
	[SerializeField] private float _lookingSensitivity = 0.1f;
	[SerializeField] private float _flySpeedMultSensitivity = 0.01f;
	
	[Header("Speed")]
	[SerializeField] private float _flySpeed = 10f;
	[SerializeField] private float _fastFlySpeed = 25f;
	[SerializeField] private Vector2 _flySpeedMultMinMax = new Vector2(0.5f, 4f);
	
	[Header("Bounds")]
	[SerializeField] private Vector2 _zoomMinMax = new Vector2(5, 50);
	[SerializeField] private Vector3 _flyBoundsCenter = Vector3.up * 25;
	[SerializeField] private Vector3 _flyBoundsSize = Vector3.one * 50;
	[SerializeField] private float _flyBoundsFalloffMax = 25;
	[SerializeField, ReadOnly] private float _inverseFalloff;
	
	[Header("References")]
	[SerializeField, HighlightIfNull] private Transform _pivot;
	[SerializeField, HighlightIfNull] private Transform _camera;

	[Header("Debug")]
	[SerializeField, ReadOnly] private bool _rotating;
	[SerializeField, ReadOnly] private bool _panning;
	[SerializeField, ReadOnly] private bool _looking;
	[SerializeField, ReadOnly] private Vector2 _mouseMovementInput;
	[SerializeField, ReadOnly] private float _zoom;
	[SerializeField, ReadOnly] private float _flySpeedMultiplier = 1;
	[SerializeField, ReadOnly] private float _flySpeedSmooth;

	public void SetCameraActive(bool active) => _camera.gameObject.SetActive(active);
	
	private void OnValidate()
	{
		if (_flyBoundsFalloffMax > 0) _inverseFalloff = 1f / _flyBoundsFalloffMax;
	}
	
	private void Start()
	{
		_zoom = -_camera.localPosition.z;
		_flySpeedMultiplier = 1;
	}

	private void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (_rotating) Rotate();
		else if (_panning) Pan();
		else if (_looking) LookAround();
	}

	public void SetRotate(bool rotate)
	{
		if (_panning || _looking) return;
		_rotating = rotate;
		HideMouse(rotate);
	}
	
	public void SetPan(bool pan)
	{
		if (_rotating || _looking) return;
		_panning = pan;
		HideMouse(pan);
	}
	
	public void SetLookAround(bool looking)
	{
		if (_panning || _rotating) return;
		_looking = looking;
		HideMouse(looking);
		if (!looking) _pivot.SetParent(transform);
		_camera.SetParent(looking ? transform : _pivot);
		if (looking) _pivot.SetParent(_camera);
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
		Move(_pivot, offset * _panningSensitivity);
	}

	public void Zoom(float amount)
	{
		if (_looking)
		{
			_flySpeedMultiplier += amount * _flySpeedMultSensitivity;
			_flySpeedMultiplier = Mathf.Clamp(_flySpeedMultiplier, _flySpeedMultMinMax.x, _flySpeedMultMinMax.y);
			return;
		}
		_zoom += -amount * _zoomingSensitivity;
		_zoom = Mathf.Clamp(_zoom, _zoomMinMax.x, _zoomMinMax.y);
		_camera.transform.localPosition = new Vector3(0, 0, -_zoom);
	}
	
	private void LookAround()
	{
		var x = _mouseMovementInput.x * _lookingSensitivity;
		var y = _mouseMovementInput.y * _lookingSensitivity;
		
		var xQuat = Quaternion.AngleAxis(x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(y, Vector3.left);

		_camera.rotation = xQuat * _camera.rotation * yQuat;
		
		var goal = (PlayerInputManager.Sprint ? _fastFlySpeed : _flySpeed) *_flySpeedMultiplier;
		_flySpeedSmooth = Mathf.Lerp(_flySpeedSmooth, goal, 10f * Time.deltaTime);
		var forward = PlayerInputManager.MoveDir.y * _flySpeedSmooth * Time.deltaTime;
		var right = PlayerInputManager.MoveDir.x * _flySpeedSmooth * Time.deltaTime;
		
		Move(_camera, _camera.forward * forward + _camera.right * right);
	}
	
	private void Move(Transform t, Vector3 moveDir)
	{
		var p = t.position;
		moveDir.x = BoundsCheck(moveDir.x, p.x, _flyBoundsCenter.x, _flyBoundsSize.x);
		moveDir.y = BoundsCheck(moveDir.y, p.y, _flyBoundsCenter.y, _flyBoundsSize.y);
		moveDir.z = BoundsCheck(moveDir.z, p.z, _flyBoundsCenter.z, _flyBoundsSize.z);
		
		t.position += moveDir;
	}
	
	private float BoundsCheck(float value, float pos, float center, float size)
	{
		// Pos Z (Forwards)
		var over = pos - center - size * 0.5f;
		if (over > 0 && value > 0) value *= Mathf.Clamp01(1 - over * _inverseFalloff);
		// Neg Z (Backwards)
		over = pos + center + size * 0.5f;
		if (over < 0 && value < 0) value *= Mathf.Clamp01(1 + over * _inverseFalloff);
		return value;
	}
	
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(_flyBoundsCenter, _flyBoundsSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(_flyBoundsCenter, _flyBoundsSize + Vector3.one * _flyBoundsFalloffMax);
	}
}
