using System;
using System.Collections.Generic;
using System.Linq;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

// Messages sent from the client (Local Connection) to the server (Remote Connections)
public enum ClientToServerId : ushort
{
	localUserCreated = 1, //
	localUserSelectCharacter, //
}

// Messages sent from the server (Remote Connections) to the client (Local Connection)
public enum ServerToClientId : ushort
{
	createRemoteUser = 1, //
	remoteUserSelectedCharacter, // 
}

public class NetworkManager : MonoBehaviour
{
	public static LocalUser LocalUser;
	public static Dictionary<ushort, RemoteUser> RemoteUsers { get; } = new Dictionary<ushort, RemoteUser>();
	public static Action OnConnected = delegate { };
	public static Action OnFailedConnection = delegate { };
    
	public Client Client { get; private set; }

	[SerializeField] private string _ip;
	[SerializeField] private ushort _port;
    
	[SerializeField] private LocalUser _localUserPrefab;
	[SerializeField] private RemoteUser _remoteUserPrefab;

    #region Singleton

	private static NetworkManager _instance;
	public static NetworkManager Instance
	{
		get => _instance;
		private set
		{
			if (_instance == null)
				_instance = value;
			else if (_instance != value)
			{
				Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
				Destroy(value);
			}
		}
	}

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

    #endregion

    #region Client Events

	private void Start()
	{
		RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

		Client = new Client();
		Client.Connected += DidConnect;
		Client.ConnectionFailed += FailedToConnect;
		Client.ClientDisconnected += PlayerLeft;
		Client.Disconnected += DidDisconnect;
		
		_ip = GameState.ServerIp;
		ushort.TryParse(GameState.ServerPort, out _port);
        
		Connect();
	}

	private void FixedUpdate()
	{
		Client.Tick();
	}

	private void OnApplicationQuit()
	{
		Client.Disconnect();

		Client.Connected -= DidConnect;
		Client.ConnectionFailed -= FailedToConnect;
		Client.ClientDisconnected -= PlayerLeft;
		Client.Disconnected -= DidDisconnect;
	}

	private void Connect()
	{
		Client.Connect($"{_ip.Trim()}:{_port}");
	}

	private void DidConnect(object sender, EventArgs e)
	{
		Debug.Log($"Connected to Server: {_ip.Trim()}:{_port}", gameObject);
		OnConnected?.Invoke();
		
		// TODO: Check if User Exists
		
		// Create Local User
		LocalUser = Instantiate(_localUserPrefab, transform);
		LocalUser.CreateUser(Client.Id, GameState.UserName);
	}

	private void FailedToConnect(object sender, EventArgs e)
	{
		Debug.LogError("Failed to Connect to Server", gameObject);
		OnFailedConnection?.Invoke();
	}

	private static void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
	{
		if (RemoteUsers.TryGetValue(e.Id, out var player))
		{
			player.DestroyUser();
			Destroy(player.gameObject);
		}
	}
	
	public static void Disconnect()
	{
		if (Instance) 
		{
			if (Instance.Client.IsConnected)
			{
				Instance.Client.Disconnect();
			}
			Destroy(Instance.gameObject);
		}
		Instance = null;
		SceneManager.LoadScene(0);
	}

	private static void DidDisconnect(object sender, EventArgs e)
	{
	}
    
    #endregion

	private static void CreateRemoteUser(ushort id, string name)
	{
		if (id == Instance.Client.Id)
		{
			Debug.LogError("Remote User Joined with ID of Client");
			return;
		}
		if (RemoteUsers.ContainsKey(id))
		{
			Debug.LogError("Remote User Joined with existing ID");
			return;
		}

		var remoteAvatar = Instantiate(Instance._remoteUserPrefab, Vector3.zero, Quaternion.identity);
		remoteAvatar.CreateUser(id, name);
		RemoteUsers.Add(id, remoteAvatar);
	}
    
    #region Messages
    
	// User Creation
	public static void OnLocalUserCreated(string name)
	{
		MessageHelper.SendStringMessage(name, ClientToServerId.localUserCreated, MessageSendMode.reliable);
	}
	[MessageHandler((ushort)ServerToClientId.createRemoteUser)]
	private static void CreateRemoteUserResponse(Message message)
	{
		var fromClientId = message.GetUShort();
		var data = message.GetString();
		CreateRemoteUser(fromClientId, data);
	}
	
	// Character Selection
	public static void OnLocalUserSelectCharacter(string character)
	{
		MessageHelper.SendStringMessage(character, ClientToServerId.localUserSelectCharacter, MessageSendMode.reliable);
	}
	[MessageHandler((ushort)ServerToClientId.remoteUserSelectedCharacter)]
	private static void RemoteUserSelectedCharacterResponse(Message message)
	{
		var fromClientId = message.GetUShort();
		var data = message.GetString();
		RemoteUsers[fromClientId].SetCharacter(data);
	}

    #endregion
}