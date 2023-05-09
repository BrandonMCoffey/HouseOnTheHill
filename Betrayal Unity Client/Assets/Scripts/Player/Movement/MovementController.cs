using UnityEngine;

public class MovementController : MonoBehaviour
{
	[SerializeField] private float _walkingSpeed = 7.5f;
	[SerializeField] private float _runningSpeed = 11.5f;
	[SerializeField] private bool _canJump = true;
	[SerializeField, ShowIf("canJump")] private float _jumpSpeed = 8.0f;
	[SerializeField] private float _gravity = 20.0f;
	[SerializeField] private Transform _cameraParent;
	[SerializeField] private float _lookSpeed = 2.0f;
	[SerializeField] private float _lookXLimit = 45.0f;
	
	[SerializeField] private CharacterController _controller;
	[SerializeField, ReadOnly] private bool _canMove = true;
	
	[SerializeField, ReadOnly] private Vector3 _moveDirection = Vector3.zero;
	private float _rotationX = 0;
	
	private float MoveSpeed => PlayerInputManager.Sprint ? _runningSpeed : _walkingSpeed;
	private bool CanJump => _canJump && _controller.isGrounded;

	public Transform CameraParent => _cameraParent;

	public void SetCanMove(bool canMove) => _canMove = canMove;

	private void OnValidate()
	{
		if (!_controller) _controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	public void MoveTo(Vector3 pos)
	{
		_controller.Move(pos - transform.position);
		SendTransformToNetwork();
	}

	public void ProcessMovement()
	{
		if (!_canMove) return;
		float movementDirectionY = _moveDirection.y;
		
		var moveDirInput = PlayerInputManager.MoveDir;
		_moveDirection = transform.forward * moveDirInput.y + transform.right * moveDirInput.x;
		_moveDirection *= MoveSpeed;

		_moveDirection.y = CanJump && PlayerInputManager.Jump ? _jumpSpeed : movementDirectionY;

		if (!_controller.isGrounded) _moveDirection.y -= _gravity * Time.deltaTime;

		_controller.Move(_moveDirection * Time.deltaTime);

		if (_canMove)
		{
			var lookDirInput = PlayerInputManager.LookDir;
			_rotationX += -lookDirInput.y * _lookSpeed;
			_rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
			_cameraParent.localRotation = Quaternion.Euler(_rotationX, 0, 0);
			transform.rotation *= Quaternion.Euler(0, lookDirInput.x * _lookSpeed, 0);
		}
		SendTransformToNetwork();
	}
	
	private void SendTransformToNetwork()
	{
		if (LocalUser.Instance) LocalUser.Instance.SetTransform(transform.position, transform.eulerAngles, _cameraParent.localEulerAngles);
	}
}
