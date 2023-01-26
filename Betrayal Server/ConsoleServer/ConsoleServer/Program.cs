using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

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

internal class Program
{
    #region Variables

    private static Server server;
    private static bool isRunning;

    private const ushort Timeout = 10000; // 10s
    private const ushort Port = 7777;

    private static List<ushort> connectedClients;
    private static Dictionary<ushort, PlayerData> playerData;
    private static List<PlayerData> leftoverPlayerData;

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

            if (input == "USERS" || input == "CONNECTED")
            {
                foreach (var client in connectedClients)
                {
                    Console.WriteLine(client); // TODO: Print All Player Data as well
                }
            }
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
        Console.WriteLine($"Client connected: ({clientId})");

        foreach (var id in connectedClients)
        {
            if (id == clientId) continue;

            Message nameMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.createRemoteUser);
            nameMessage.AddUShort(id);
            nameMessage.AddString(playerData[id].Name);
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
        leftoverPlayerData.Add(playerData[clientId]);
        playerData.Remove(clientId);
        connectedClients.Remove(clientId);
        Console.WriteLine($"Client disconnected ({e.Id})");
    }

    #endregion

    [MessageHandler((ushort)ClientToServerId.clientConnectedToServer)]
    private static void HandleClientConnectedToServer(ushort fromClientId, Message message)
    {
        string name = message.GetString();
        Console.WriteLine($"User ({fromClientId}) Joined with Name ({name})");

        playerData[fromClientId].Name = name;
        SendStringMessage(fromClientId, name, ServerToClientId.createRemoteUser, MessageSendMode.reliable);
    }

    [MessageHandler((ushort)ClientToServerId.localUserSelectCharacter)]
    private static void HandleLocalUserSelectCharacter(ushort fromClientId, Message message)
    {
        int character = message.GetInt();
        Console.WriteLine($"User ({fromClientId}) Selected Character ({character})");

        playerData[fromClientId].Character = character;
        SendIntMessage(fromClientId, character, ServerToClientId.remoteUserSelectCharacter, MessageSendMode.reliable);
    }

    [MessageHandler((ushort)ClientToServerId.localUserReadyUp)]
    private static void HandleLocalUserReadyUp(ushort fromClientId, Message message)
    {
        bool ready = message.GetBool();
        Console.WriteLine($"User ({fromClientId}) is now ({(ready ? "Ready" : "Not Ready")})");

        playerData[fromClientId].Ready = ready;
        SendBoolMessage(fromClientId, ready, ServerToClientId.remoteUserReadyUp, MessageSendMode.reliable);
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
        foreach (var client in connectedClients)
        {
            if (client == fromClientId)
                continue;
            server.Send(message, client);
        }
    }

    #endregion
}

internal class PlayerData
{
    public string Name = "Player";
    public int Character = -1;
    public bool Ready = false;
}
