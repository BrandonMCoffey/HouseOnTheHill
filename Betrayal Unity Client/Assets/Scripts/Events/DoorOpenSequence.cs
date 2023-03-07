using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSequence : MonoBehaviour
{
	[SerializeField] private RoomController _roomController;
	[SerializeField] private Animator _animator;
	[SerializeField] private GameObject _animationCam;
	[SerializeField] private Transform _doorPivotRef;
	
	private void Awake()
	{
		_animationCam.SetActive(false);
	}
	
	[Button]
	public void PlaySequence(DoorController door, PlayerActionManager player)
	{
		StartCoroutine(AnimateDoorRoutine(door, player));
	}
	
	private IEnumerator AnimateDoorRoutine(DoorController door, PlayerActionManager player)
	{
		player.SetPlayerEnabled(false);
		_animator.SetTrigger("PlayAnimation");
		_animator.ResetTrigger("StopAnimation");
		_animationCam.SetActive(true);
		var t = door.transform;
		transform.SetPositionAndRotation(t.position, t.rotation);
		while (true)
		{
			var time = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if (time >= 1f) break;
			door.Pivot.rotation = _doorPivotRef.rotation;
			yield return null;
		}
		_animationCam.SetActive(false);
		var p = _animationCam.transform.position;
		player.transform.position = new Vector3(p.x, player.transform.position.y, p.z);
		player.SetPlayerEnabled(true, false);
		_animator.SetTrigger("StopAnimation");
		_roomController.OpenAllConnectedDoors(false);
	}
}
