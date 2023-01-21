using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private TMP_InputField _name;
	[SerializeField] private ToggleableInputField _ip;
	[SerializeField] private ToggleableInputField _port;
	
	public void ConnectToServer()
	{
		// TODO: Check Valid Name is Set
		GameState.UserName = _name.text;
		GameState.ServerIp = _ip.Value;
		GameState.ServerPort = _port.Value;
		SceneManager.LoadScene(1);
	}
	
	public void QuitGame()
	{
		Application.Quit();
		
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}
