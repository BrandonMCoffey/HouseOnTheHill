using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

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

internal enum GameState
{
    lobby,
    lobbyAllReady,
    loadingGame,
    playingGame,
}

internal class PlayerData
{
    public string UserName = "Player";

    public bool Ready;
    public bool GameLoaded;
    public int Character = -1;

    public int SpeedIndex = -1;
    public int MightIndex = -1;
    public int SanityIndex = -1;
    public int KnowledgeIndex = -1;

    public List<int> ItemIdsHeld = new List<int>();

    public int RoomIndex;
    public Vector3 RoomOffset;
    public Vector3 Rotation;

    public string PrintInfo() => $"- Name: {UserName}, {(Ready ? "Is Ready" : "Is Not Ready")}\n" +
                                 $"- Character: {Character} with trait indexes {SpeedIndex},{MightIndex},{SanityIndex},{KnowledgeIndex}\n" +
                                 $"- Position: Room {RoomIndex} + with offset {RoomOffset} and rotation {Rotation}";
}

internal class Program
{
    #region Variables

    private static Server server;
    private static bool isRunning;

    private const ushort Timeout = 10000; // 10s
    private const ushort Port = 7777;

    private const float LobbyCountdown = 5;

    private static List<ushort> connectedClients;
    private static Dictionary<ushort, PlayerData> playerData;
    private static List<PlayerData> leftoverPlayerData;

    private static GameState gameState = GameState.lobby;

    private static float lobbyCountdownTimer;
    private static List<ushort> turnOrder;
    private static int turnOrderIndex;

    private static bool GameStarted => gameState == GameState.playingGame;

    #endregion

    #region Main Networking Loop

    private static void Main()
    {
        Console.Title = "Server";

        RiptideLogger.Initialize(Console.WriteLine, true);
        isRunning = true;

        new Thread(Loop).Start();

        Console.WriteLine("Write QUIT to stop the server at any time.");
        while (true)
        {
            string input = Console.ReadLine()?.Trim().ToUpper();
            if (input == "QUIT" || input == "STOP") break;
            if (input == "USERS" || input == "CONNECTED") PrintUserInfo();
        }

        isRunning = false;

        Console.ReadLine();
    }

    private static void Loop()
    {
        server = new Server
        {
            TimeoutTime = Timeout
        };
        server.Start(Port, 6);

        connectedClients = new List<ushort>();
        playerData = new Dictionary<ushort, PlayerData>();
        leftoverPlayerData = new List<PlayerData>();

        server.ClientConnected += NewPlayerConnected;
        server.ClientDisconnected += PlayerLeft;

        while (isRunning)
        {
            switch (gameState)
            {
                case GameState.lobby:
                    break;
                case GameState.lobbyAllReady:
                    lobbyCountdownTimer -= 0.01f;
                    if (lobbyCountdownTimer <= 0)
                    {
                        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.loadGameScene);
                        SendMessageToAll(message);
                        gameState = GameState.loadingGame;
                    }
                    break;
                case GameState.loadingGame:
                    break;
                case GameState.playingGame:
                    break;
            }

            server.Tick();
            Thread.Sleep(10);
        }

        server.ClientConnected -= NewPlayerConnected;
        server.ClientDisconnected -= PlayerLeft;

        server.Stop();
    }

    private static void NewPlayerConnected(object sender, ServerClientConnectedEventArgs e)
    {
        var clientId = e.Client.Id;
        connectedClients.Add(clientId);
        playerData.Add(clientId, new PlayerData());
        CheckAllPlayersReady();

        Console.WriteLine($"Client connected: ({clientId})");

        foreach (var id in connectedClients)
        {
            if (id == clientId) continue;

            Message nameMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.createRemoteUser);
            nameMessage.AddUShort(id);
            nameMessage.AddString(playerData[id].UserName);
            server.Send(nameMessage, clientId);

            Message characterMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.remoteUserSelectCharacter);
            characterMessage.AddUShort(id);
            characterMessage.AddInt(playerData[id].Character);
            server.Send(characterMessage, clientId);

            Message readyMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.remoteUserReadyUp);
            readyMessage.AddUShort(id);
            readyMessage.AddBool(playerData[id].Ready);
            server.Send(readyMessage, clientId);
        }
    }

    private static void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        var clientId = e.Id;
        if (GameStarted) leftoverPlayerData.Add(playerData[clientId]);
        connectedClients.Remove(clientId);
        playerData.Remove(clientId);
        CheckAllPlayersReady();

        Console.WriteLine($"Client disconnected ({e.Id})");
    }

    #endregion

    [MessageHandler((ushort)ClientToServerId.clientConnectedToServer)]
    private static void HandleClientConnectedToServer(ushort fromClientId, Message message)
    {
        string name = message.GetString();

        playerData[fromClientId].UserName = name;
        SendStringMessage(fromClientId, name, ServerToClientId.createRemoteUser, MessageSendMode.reliable);

        PrintUserEvent(fromClientId, "Joined the Lobby");
    }

    [MessageHandler((ushort)ClientToServerId.localUserSelectCharacter)]
    private static void HandleLocalUserSelectCharacter(ushort fromClientId, Message message)
    {
        int character = message.GetInt();

        playerData[fromClientId].Character = character;
        SendIntMessage(fromClientId, character, ServerToClientId.remoteUserSelectCharacter, MessageSendMode.reliable);

        PrintUserEvent(fromClientId, $"Selected Character ({character})");
    }

    [MessageHandler((ushort)ClientToServerId.localUserReadyUp)]
    private static void HandleLocalUserReadyUp(ushort fromClientId, Message message)
    {
        bool ready = message.GetBool();

        playerData[fromClientId].Ready = ready;
        CheckAllPlayersReady();
        SendBoolMessage(fromClientId, ready, ServerToClientId.remoteUserReadyUp, MessageSendMode.reliable);

        PrintUserEvent(fromClientId, $"is now ({(ready ? "Ready" : "Not Ready")})");
    }

    private static void CheckAllPlayersReady()
    {
        if (GameStarted)
            return;
        int ready = playerData.Sum(pair => pair.Value.Ready ? 1 : 0);
        int total = playerData.Count;
        var allReady = ready == total;

        gameState = allReady ? GameState.lobbyAllReady : GameState.lobby;
        lobbyCountdownTimer = LobbyCountdown;

        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.lobbyTimerCountdown);
        message.AddBool(allReady);
        message.AddFloat(LobbyCountdown);
        SendMessageToAll(message);

        Console.WriteLine(allReady ? $"All Players Ready! Starting Game in {LobbyCountdown} seconds." : $"Players Ready ({ready}/{total})");
    }

    [MessageHandler((ushort)ClientToServerId.gameLoaded)]
    private static void HandleLocalUserGameLoaded(ushort fromClientId, Message message)
    {
        playerData[fromClientId].GameLoaded = true;
        CheckAllPlayersLoaded();
    }

    private static void CheckAllPlayersLoaded()
    {
        bool allLoaded = playerData.Aggregate(true, (current, pair) => current & pair.Value.GameLoaded);
        if (allLoaded)
        {
            gameState = GameState.playingGame;
            turnOrderIndex = 0;
            turnOrder = new List<ushort>();
            foreach ((ushort clientId, PlayerData playerData) in playerData)
            {
                if (playerData.Character > 0) turnOrder.Add(clientId);
            }

            Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.setupGame);
            SendMessageToAll(message);

            Console.WriteLine("All Players Loaded. Setting Up Game!");
        }
    }

    [MessageHandler((ushort)ClientToServerId.updateLocalUserTransform)]
    private static void HandleUpdateLocalUserTransform(ushort fromClientId, Message message)
    {
        var data = message.GetFloats(6);
        SendTransformMessage(fromClientId, data, ServerToClientId.updateRemoteUserTransform, MessageSendMode.unreliable);
    }

    [MessageHandler((ushort)ClientToServerId.createNewRoom)]
    private static void HandleCreateNewRoom(ushort fromClientId, Message message)
    {
        var roomId = message.GetInt();
        var data = message.GetFloats(6);

        PrintUserEvent(fromClientId, $"New Room Created ({roomId})");

        Message sendMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.receiveRoomCreated);
        sendMessage.AddUShort(fromClientId);
        sendMessage.AddInt(roomId);
        sendMessage.AddFloats(data, false);
        SendMessage(sendMessage, fromClientId);
    }

    #region Helper Functions

    private static void SendBoolMessage(ushort fromClientId, bool data, ServerToClientId messageType, MessageSendMode sendMode)
    {
        Message message = Message.Create(sendMode, messageType);
        message.AddUShort(fromClientId);
        message.AddBool(data);
        SendMessage(message, fromClientId);
    }

    private static void SendIntMessage(ushort fromClientId, int data, ServerToClientId messageType, MessageSendMode sendMode)
    {
        Message message = Message.Create(sendMode, messageType);
        message.AddUShort(fromClientId);
        message.AddInt(data);
        SendMessage(message, fromClientId);
    }

    private static void SendStringMessage(ushort fromClientId, string data, ServerToClientId messageType, MessageSendMode sendMode)
    {
        Message message = Message.Create(sendMode, messageType);
        message.AddUShort(fromClientId);
        message.AddString(data);
        SendMessage(message, fromClientId);
    }

    private static void SendTransformMessage(ushort fromClientId, float[] data, ServerToClientId messageType, MessageSendMode sendMode)
    {
        Message message = Message.Create(sendMode, messageType);
        message.AddUShort(fromClientId);
        message.AddFloats(data, false);
        SendMessage(message, fromClientId);
    }

    private static void SendRigidbodyMessage(ushort fromClientId, int objId, float[] data, ServerToClientId messageType, MessageSendMode sendMode)
    {
        Message message = Message.Create(sendMode, messageType);
        message.AddInt(objId);
        message.AddFloats(data, false);
        SendMessage(message, fromClientId);
    }

    private static void SendMessage(Message message, ushort fromClientId)
    {
        foreach (ushort client in connectedClients.Where(client => client != fromClientId)) server.Send(message, client);
    }

    private static void SendMessageToAll(Message message)
    {
        foreach (ushort client in connectedClients) server.Send(message, client);
    }

    #endregion

    private static void PrintUserEvent(ushort user, string message)
    {
        Console.WriteLine($"({user}) {playerData[user].UserName} {message}");
    }

    private static void PrintUserInfo()
    {
        foreach (var client in connectedClients)
        {
            Console.WriteLine(client); // TODO: Print All Player Data as well
        }
        foreach ((ushort key, PlayerData user) in playerData)
        {
            Console.WriteLine($"{key}\n{user.PrintInfo()}");
        }
    }
}
