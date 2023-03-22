using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSequence : MonoBehaviour
{
	[SerializeField] private PlayerManager _playerManager;
	[SerializeField] private PlayerActionManager _playerActions;
	[SerializeField] private RoomController _roomController;
	[SerializeField] private Animator _animator;
	[SerializeField] private GameObject _animationCam;
	[SerializeField] private Transform _doorPivotRef;
	
	private void Awake()
	{
		_animationCam.SetActive(false);
	}
	
	public void PlaySequence(DoorController door)
	{
		StartCoroutine(AnimateDoorRoutine(door));
	}
	
	private IEnumerator AnimateDoorRoutine(DoorController door)
	{
		_playerActions.SetPlayerEnabled(false);
		_playerManager.SetIgnoreInput(true);
		_animator.SetTrigger("PlayAnimation");
		_animator.ResetTrigger("StopAnimation");
		_animationCam.SetActive(true);
		var t = door.transform;
		Vector3 pos = _playerActions.PlayerMovement.transform.position;
		transform.SetPositionAndRotation(t.position, t.rotation);
		while (true)
		{
			var time = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if (time >= 1f) break;
			door.Pivot.rotation = _doorPivotRef.rotation;
			var camPos = _animationCam.transform.position;
			pos.x = camPos.x;
			pos.z = camPos.z;
			_playerActions.PlayerMovement.MoveTo(pos);
			yield return null;
		}
		_animationCam.SetActive(false);
		_playerActions.SetPlayerEnabled(true);
		_playerManager.SetIgnoreInput(false);
		_animator.SetTrigger("StopAnimation");
		_roomController.OpenAllConnectedDoors(false);
	}
}
