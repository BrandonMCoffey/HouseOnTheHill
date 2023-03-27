using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	private static GameData _instance;
	public static GameData Instance
	{
		get 
		{
			if (_instance) return _instance;
			_instance = FindObjectOfType<GameData>();
			return _instance;
		}
	}
	
	[SerializeField] private string _userName = "Player";
	[SerializeField] private string _serverIp = "127.0.0.1";
	[SerializeField] private ushort _serverPort = 7777;
	[SerializeField] private List<Character> _characters = new List<Character>();
	
	[SerializeField, ReadOnly] private bool _gameStarted;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		DontDestroyOnLoad(this);
	}
	
	public static string UserName {
		get { return Instance._userName; }
		set { if (!string.IsNullOrEmpty(value))Instance._userName = value; }
	}
	public static string ServerIp {
		get { return Instance._serverIp; }
		set { if (!string.IsNullOrEmpty(value)) Instance._serverIp = value; }
	}
	public static ushort ServerPort {
		get { return Instance._serverPort; }
		set { Instance._serverPort = value; }
	}
	
	public static int CharacterCount => Instance._characters.Count;
	public static Character GetCharacter(int index) => index < 0 ? null : Instance._characters[index];
	public static int GetCharacterIndex(Character character) => Instance._characters.IndexOf(character);
	
	public static bool GameStarted => Instance._gameStarted;
}
