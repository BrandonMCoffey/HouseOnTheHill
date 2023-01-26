using System;
using System.Collections.Generic;
using System.Linq;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

// Messages sent from the client (Local) to the server (Remote)
public enum ClientToServerId : ushort
{
	clientConnectedToServer = 1, // reliable (string userName)
	localUserSelectCharacter, // reliable (int characterIndex)
	localUserReadyUp, // reliable (bool ready)

	updateLocalUserTraits, // reliable (int[4] traitValues)
	updateLocalUserItemsHeld, // reliable (int[] itemIds)
	updateLocalUserTransform, // unreliable (int roomId, float[6] TransformData)

	createNewRoom, // reliable (int roomId, int floor, int x, int y, int rotation)
	sendAnnouncement, // reliable (string title, string text)

	localUserEndTurn, // reliable
}

// Messages sent from the server (Remote) to the client (Local)
public enum ServerToClientId : ushort
{
	createRemoteUser = 1, // reliable (string userName)
	remoteUserSelectCharacter, // reliable (int characterIndex)
	remoteUserReadyUp, // reliable (bool ready)

	updateRemoteUserTraits, // reliable (int[4] traitValues)
	updateRemoteUserItemsHeld, // reliable (int[] itemIds)
	updateRemoteUserTransform, // unreliable (int roomId, float[6] TransformData)

	receiveRoomCreated, // reliable (int roomId, int floor, int x, int y, int rotation)
	receiveAnnouncement, // reliable (string title, string text)

	updateCurrentPlayerTurn, // reliable (ushort usersTurn)
}

public class NetworkManager : MonoBehaviour
{
	public static Action OnConnected = delegate { };
	public static Action OnFailedConnection = delegate { };
	public static Action OnDidDisconnect = delegate { };
    
	private static Dictionary<ushort, User> RemoteUsers;

	public Client Client { get; private set; }

	[SerializeField] private string _ip;
	[SerializeField] private ushort _port;
    
	[SerializeField] private LocalUser _localUserPrefab;
	[SerializeField] private User _remoteUserPrefab;

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
        
		RemoteUsers = new Dictionary<ushort, User>();
		
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

	public void Connect()
	{
		_ip = GameState.ServerIp;
		_port = GameState.ServerPort;
		
		Client.Connect($"{_ip.Trim()}:{_port}");
	}

	private void DidConnect(object sender, EventArgs e)
	{
		Debug.Log($"Connected to Server: {_ip.Trim()}:{_port}", gameObject);
		OnConnected?.Invoke();
		
		// TODO: Check if User Exists
		
		// Create Local User
		var localUser = Instantiate(_localUserPrefab, transform);
		localUser.User.CreateUser(Client.Id, true, GameState.UserName);
		NetworkManager.OnLocalUserCreated(GameState.UserName);
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
		_instance = null;
		SceneManager.LoadScene(0);
	}

	private static void DidDisconnect(object sender, EventArgs e)
	{
		OnDidDisconnect?.Invoke();
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
			Debug.LogError($"Remote User Joined with existing ID ({id})");
			return;
		}

		var remoteUser = Instantiate(Instance._remoteUserPrefab, Instance.transform);
		remoteUser.CreateUser(id, false, name);
		RemoteUsers.Add(id, remoteUser);
	}
    
    #region Messages
    
	// User Creation
	public static void OnLocalUserCreated(string name)
	{
		MessageHelper.SendStringMessage(name, ClientToServerId.clientConnectedToServer, MessageSendMode.reliable);
	}
	[MessageHandler((ushort)ServerToClientId.createRemoteUser)]
	private static void CreateRemoteUserResponse(Message message)
	{
		var fromClientId = message.GetUShort();
		var data = message.GetString();
		CreateRemoteUser(fromClientId, data);
	}
	
	// Character Selection
	public static void OnLocalUserSelectCharacter(int character)
	{
		MessageHelper.SendIntMessage(character, ClientToServerId.localUserSelectCharacter, MessageSendMode.reliable);
	}
	[MessageHandler((ushort)ServerToClientId.remoteUserSelectCharacter)]
	private static void RemoteUserSelectedCharacterResponse(Message message)
	{
		var fromClientId = message.GetUShort();
		var data = message.GetInt();
		RemoteUsers[fromClientId].SetCharacter(data);
	}
	
	// Ready Up
	public static void OnLocalUserReadyUp(bool ready)
	{
		MessageHelper.SendBoolMessage(ready, ClientToServerId.localUserReadyUp, MessageSendMode.reliable);
	}
	[MessageHandler((ushort)ServerToClientId.remoteUserReadyUp)]
	private static void RemoteUserReadyUpResponse(Message message)
	{
		var fromClientId = message.GetUShort();
		var data = message.GetBool();
		RemoteUsers[fromClientId].SetReady(data);
	}

    #endregion
}