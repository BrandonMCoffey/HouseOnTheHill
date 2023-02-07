using System.Collections;
using System.Collections.Generic;
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
	
	[SerializeField, ReadOnly] private Vector2 _moveDirectionInput;
	[SerializeField, ReadOnly] private bool _sprintInput;
	[SerializeField, ReadOnly] private bool _jumpInput;
	[SerializeField, ReadOnly] private Vector2 _lookDirectionInput;
	[SerializeField, ReadOnly] private Vector3 _moveDirection = Vector3.zero;
	private float rotationX = 0;
	
	private float MoveSpeed => _sprintInput ? _runningSpeed : _walkingSpeed;
	private bool CanJump => _canJump && _controller.isGrounded;

	public void SetCanMove(bool canMove) => _canMove = canMove;
	public void SetSprinting(bool sprint) => _sprintInput = sprint;
	public void SetMoveDir(Vector2 dir) => _moveDirectionInput = dir;
	public void SetLookDir(Vector2 dir) => _lookDirectionInput = dir;
	public void SetJumpThisFrame() => _jumpInput = true;

	private void OnValidate()
	{
		if (!_controller) _controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		if (!_canMove) return;
		float movementDirectionY = _moveDirection.y;
		
		_moveDirection = transform.forward * _moveDirectionInput.y + transform.right * _moveDirectionInput.x;
		_moveDirection *= MoveSpeed;

		_moveDirection.y = CanJump && _jumpInput ? _jumpSpeed : movementDirectionY;
		_jumpInput = false;

		if (!_controller.isGrounded) _moveDirection.y -= _gravity * Time.deltaTime;

		_controller.Move(_moveDirection * Time.deltaTime);

		if (_canMove)
		{
			rotationX += -_lookDirectionInput.y * _lookSpeed;
			rotationX = Mathf.Clamp(rotationX, -_lookXLimit, _lookXLimit);
			_cameraParent.localRotation = Quaternion.Euler(rotationX, 0, 0);
			transform.rotation *= Quaternion.Euler(0, _lookDirectionInput.x * _lookSpeed, 0);
		}
		SendTransformToNetwork();
	}
	
	private void SendTransformToNetwork()
	{
		if (LocalUser.Instance) LocalUser.Instance.SetTransform(transform.position, transform.eulerAngles);
	}
}
