using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	[SerializeField] private float walkingSpeed = 7.5f;
	[SerializeField] private float runningSpeed = 11.5f;
	[SerializeField] private float jumpSpeed = 8.0f;
	[SerializeField] private float gravity = 20.0f;
	[SerializeField] private Transform _cameraParent;
	[SerializeField] private float lookSpeed = 2.0f;
	[SerializeField] private float lookXLimit = 45.0f;
	
	[SerializeField] private CharacterController _controller;
	[SerializeField, ReadOnly] private bool canMove = true;

	private Vector3 moveDirection = Vector3.zero;
	private float rotationX = 0;

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
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		Vector3 right = transform.TransformDirection(Vector3.right);
		// Press Left Shift to run
		bool isRunning = Input.GetKey(KeyCode.LeftShift);
		float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
		float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
		float movementDirectionY = moveDirection.y;
		moveDirection = (forward * curSpeedX) + (right * curSpeedY);

		if (Input.GetButton("Jump") && canMove && _controller.isGrounded)
		{
			moveDirection.y = jumpSpeed;
		}
		else
		{
			moveDirection.y = movementDirectionY;
		}

		// Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
		// when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
		// as an acceleration (ms^-2)
		if (!_controller.isGrounded)
		{
			moveDirection.y -= gravity * Time.deltaTime;
		}

		// Move the controller
		_controller.Move(moveDirection * Time.deltaTime);

		// Player and Camera rotation
		if (canMove)
		{
			rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
			rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
			_cameraParent.localRotation = Quaternion.Euler(rotationX, 0, 0);
			transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
		}
	}
}
