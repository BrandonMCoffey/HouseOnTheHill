using System;
using System.Collections.Generic;
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

	gameLoaded, // reliable

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
	createRemoteUser = 1, // reliable (ushort client, string userName)
	remoteUserSelectCharacter, // reliable (ushort client, int characterIndex)
	remoteUserReadyUp, // reliable (ushort client, bool ready)

	lobbyTimerCountdown, // reliable (bool countdown, float countdownTime)
	loadGameScene, // reliable
	setupGame, // reliable (???)

	updateRemoteUserTraits, // reliable (ushort client, int[4] traitValues)
	updateRemoteUserItemsHeld, // reliable (ushort client, int[] itemIds)
	updateRemoteUserTransform, // unreliable (ushort client, int roomId, float[6] TransformData)

	receiveRoomCreated, // reliable (ushort client, int roomId, int floor, int x, int y, int rotation)
	receiveAnnouncement, // reliable (ushort client, string title, string text)

	updateCurrentPlayerTurn, // reliable (ushort fromId, ushort usersTurn)
}

public class NetworkManager : MonoBehaviour
{
	public static Action OnConnected = delegate { };
	public static Action OnFailedConnection = delegate { };
	public static Action OnDidDisconnect = delegate { };
    
	public static Dictionary<ushort, User> AllUsers;
	public static Action OnUpdateAllUsers = delegate { };
	public static Action<User> OnSetUsersTurn = delegate { };

	public Client Client { get; private set; }

	[SerializeField] private bool _debug;

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
				Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
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
        
		AllUsers = new Dictionary<ushort, User>();
		
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
		_ip = GameData.ServerIp;
		_port = GameData.ServerPort;
		
		Client.Connect($"{_ip.Trim()}:{_port}");
	}

	private void DidConnect(object sender, EventArgs e)
	{
		Log($"Connected to Server: {_ip.Trim()}:{_port}");
		OnConnected?.Invoke();
		
		// TODO: Check if User Exists
		
		// Create Local User
		var localUser = Instantiate(_localUserPrefab, transform);
		localUser.CreateUser(Client.Id, true, GameData.UserName);
		AllUsers.Add(Client.Id, localUser);
		OnUpdateAllUsers?.Invoke();
		NetworkManager.OnLocalUserCreated(GameData.UserName);
	}

	private void FailedToConnect(object sender, EventArgs e)
	{
		Log("Failed to Connect to Server");
		OnFailedConnection?.Invoke();
	}

	private static void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
	{
		if (AllUsers.TryGetValue(e.Id, out var player))
		{
			player.DestroyUser();
			Destroy(player.gameObject);
			AllUsers.Remove(e.Id);
			OnUpdateAllUsers?.Invoke();
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
			LogError("Remote User Joined with ID of Client");
			return;
		}
		if (AllUsers.ContainsKey(id))
		{
			LogError($"Remote User Joined with existing ID ({id})");
			return;
		}

		var remoteUser = Instantiate(Instance._remoteUserPrefab, Instance.transform);
		remoteUser.CreateUser(id, false, name);
		AllUsers.Add(id, remoteUser);
		OnUpdateAllUsers?.Invoke();
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
		LogUser(fromClientId, $"Selected Character {data}");
		AllUsers[fromClientId].SetCharacter(data);
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
		LogUser(fromClientId, $"Is {(data ? "Ready" : "Not Ready")}");
		AllUsers[fromClientId].SetReady(data);
	}
	
	[MessageHandler((ushort)ServerToClientId.lobbyTimerCountdown)]
	private static void LobbyCountdownTimerResponse(Message message)
	{
		var runTimer = message.GetBool();
		var timerLength = message.GetFloat();
		if (runTimer) LobbyController.StartCountdown(timerLength);
		else LobbyController.StopCountdown();
	}
	
	// Game Loaded
	[MessageHandler((ushort)ServerToClientId.loadGameScene)]
	private static void LoadGameSceneResponse(Message message)
	{
		Log("Load Game");
		SceneManager.LoadScene("Game");
	}
	public static void OnGameLoaded()
	{
		MessageHelper.SendEmptyMessage(ClientToServerId.gameLoaded, MessageSendMode.reliable);
	}
	[MessageHandler((ushort)ServerToClientId.setupGame)]
	private static void SetupGameResponse(Message message)
	{
		var turnOrder = message.GetUShorts();
		SetCurrentPlayerTurn(turnOrder[0]);
		Log("Setup Game!");
	}
	
	// Player Transform
	public static void OnUpdateLocalUserTransformCharacter(Vector3 pos, Vector3 rot, Vector3 cameraRot)
	{
		MessageHelper.SendPlayerTransformMessage(pos, rot, cameraRot, ClientToServerId.updateLocalUserTransform, MessageSendMode.unreliable);
	}
	[MessageHandler((ushort)ServerToClientId.updateRemoteUserTransform)]
	private static void UpdateRemoteUserTransformResponse(Message message)
	{
		var fromClientId = message.GetUShort();
		var pos = message.GetVector3();
		var rot = message.GetVector3();
		var cameraRot = message.GetVector3();
		AllUsers[fromClientId].SetTransform(pos, rot, cameraRot);
	}
	
	// Room Generation
	public static void OnCreateNewRoomLocally(int roomId, int floor, int x, int z, int rot)
	{
		var message = Message.Create(MessageSendMode.reliable, ClientToServerId.createNewRoom);
		message.Add(roomId);
		message.Add(floor);
		message.Add(x);
		message.Add(z);
		message.Add(rot);
		Instance.Client.Send(message);
	}
	[MessageHandler((ushort)ServerToClientId.receiveRoomCreated)]
	private static void CreateNewRoomRemotelyResponse(Message message)
	{
		var fromClientId = message.GetUShort();
		var roomId = message.GetInt();
		var floor = message.GetInt();
		var x = message.GetInt();
		var z = message.GetInt();
		var rot = message.GetInt();
		
		LogUser(fromClientId, $"Create Room {roomId} on floor {floor} at pos {x},{z} with rot {rot}");
		
		RoomController.CreateRoomRemotely(roomId, floor, x, z, rot);
	}

	// Turns
	public static void OnLocalUserEndTurn()
	{
		MessageHelper.SendEmptyMessage(ClientToServerId.localUserEndTurn, MessageSendMode.reliable);
	}

	[MessageHandler((ushort)ServerToClientId.updateCurrentPlayerTurn)]
	private static void UpdateCurrentPlayerTurnResponse(Message message)
	{
		var usersTurn = message.GetUShort();
		SetCurrentPlayerTurn(usersTurn);
	}

	private static void SetCurrentPlayerTurn(ushort playersTurn)
	{
		foreach (var pair in AllUsers)
		{
			pair.Value.SetCurrentTurn(pair.Key == playersTurn);
		}
		OnSetUsersTurn?.Invoke(AllUsers[playersTurn]);
	}

	#endregion
    
	private static void LogUser(ushort id, string message)
	{
		Log($"({id}) {AllUsers[id].UserName} {message}");
	}
    
	private static void Log(string message)
	{
		if (Instance._debug) Debug.Log(message, Instance.gameObject);
	}
	
	private static void LogError(string message)
	{
		if (Instance._debug) Debug.LogError(message, Instance.gameObject);
	}
}