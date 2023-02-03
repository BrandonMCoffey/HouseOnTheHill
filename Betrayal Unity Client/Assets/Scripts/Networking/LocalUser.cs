using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUser : MonoBehaviour
{
	public static LocalUser Instance;
	
	[SerializeField, ReadOnly] private User _user;
	
	public User User => _user;
	
	private void Awake()
	{
		Instance = this;
	}
	
	private void OnValidate()
	{
		if (!_user)
		{
			_user = GetComponent<User>();
			if (!_user) _user = gameObject.AddComponent<User>();
		}
	}
	
	public static void SetCharacter(int character)
	{
		Instance.User.SetCharacter(character);
		NetworkManager.OnLocalUserSelectCharacter(character);
	}
	
	public static void SetReady(bool ready)
	{
		Instance.User.SetReady(ready);
		NetworkManager.OnLocalUserReadyUp(ready);
	}
	
	public static void SetTransform(Vector3 pos, Vector3 rot)
	{
		Instance.User.SetTransform(pos, rot, false);
		NetworkManager.OnUpdateLocalUserTransformCharacter(pos, rot);
	}
}
