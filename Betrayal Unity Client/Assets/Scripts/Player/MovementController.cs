using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	[SerializeField] private float walkingSpeed = 7.5f;
	[SerializeField] private float runningSpeed = 11.5f;
	[SerializeField] private bool canJump = true;
	[SerializeField, ShowIf("canJump")] private float jumpSpeed = 8.0f;
	[SerializeField] private float gravity = 20.0f;
	[SerializeField] private Transform _cameraParent;
	[SerializeField] private float lookSpeed = 2.0f;
	[SerializeField] private float lookXLimit = 45.0f;
	
	[SerializeField] private CharacterController _controller;
	[SerializeField, ReadOnly] private bool canMove = true;

	[SerializeField, ReadOnly] private Vector3 moveDirection = Vector3.zero;
	private float rotationX = 0;
	
	private bool IsRunning => Input.GetKey(KeyCode.LeftShift);
	private float MoveSpeed => IsRunning ? runningSpeed : walkingSpeed;
	private Vector2 MoveDir => new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
	private bool CanJump => canJump && _controller.isGrounded;
	private bool JumpThisFrame => Input.GetButton("Jump");
	private Vector2 LookDir => new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

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
		if (!canMove) return;
		float movementDirectionY = moveDirection.y;
		
		var movement = MoveDir * MoveSpeed;
		moveDirection = (transform.forward * movement.x) + (transform.right * movement.y);

		moveDirection.y = CanJump && JumpThisFrame ? jumpSpeed : movementDirectionY;

		if (!_controller.isGrounded) moveDirection.y -= gravity * Time.deltaTime;

		_controller.Move(moveDirection * Time.deltaTime);

		if (canMove)
		{
			var lookDir = LookDir;
			rotationX += -lookDir.x * lookSpeed;
			rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
			_cameraParent.localRotation = Quaternion.Euler(rotationX, 0, 0);
			transform.rotation *= Quaternion.Euler(0, lookDir.y * lookSpeed, 0);
		}
		SendTransformToNetwork();
	}
	
	private void SendTransformToNetwork()
	{
		if (NetworkManager.Instance) LocalUser.SetTransform(transform.position, transform.eulerAngles);
	}
}
