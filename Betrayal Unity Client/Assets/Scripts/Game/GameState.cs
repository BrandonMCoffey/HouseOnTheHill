using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
	public static GameState Instance;
	
	public static string UserName = "Player";
	
	public static string ServerIp = "127.0.0.1";
	public static string ServerPort = "7777";
	
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
	}
}
