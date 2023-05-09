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
		GameData.UserName = _name.text;
		GameData.ServerIp = _ip.Value;
		if (ushort.TryParse(_port.Value, out var port))
		{
			GameData.ServerPort = port;
		}
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
