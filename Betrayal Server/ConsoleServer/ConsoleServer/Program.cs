using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

// Messages sent from the client (Local Connection) to the server (Remote Connections)
public enum ClientToServerId : ushort
{
    localUserCreated = 1, //
    localUserSelectCharacter, // 
}

// Messages sent from the server (Remote Connections) to the client (Local Connection)
public enum ServerToClientId : ushort
{
    createRemoteUser = 1, // Tells all clients that a new player has joined
    remoteUserSelectedCharacter, // 
}

internal class Program
{
    private static Server server;
    private static bool isRunning;

    private const ushort Timeout = 10000; // 10s
    private const ushort Port = 7777;

    private static List<ushort> connectedClients;

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
            string input = Console.ReadLine().Trim().ToUpper();
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
        server.Start(Port, 10);

        connectedClients = new List<ushort>();
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
        SendStringMessage(clientId, "", ServerToClientId.createRemoteUser, MessageSendMode.reliable);
        Console.WriteLine($"Client connected: ({clientId})");
    }

    private static void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        connectedClients.Remove(e.Id);
        Console.WriteLine($"Client disconnected ({e.Id})");
    }

    #endregion

    [MessageHandler((ushort)ClientToServerId.localUserCreated)]
    private static void HandleUserCreated(ushort fromClientId, Message message)
    {
        string name = message.GetString();
        Console.WriteLine($"User ({fromClientId}) Joined with Name ({name})");
        SendStringMessage(fromClientId, name, ServerToClientId.createRemoteUser, MessageSendMode.reliable);
    }

    [MessageHandler((ushort)ClientToServerId.localUserSelectCharacter)]
    private static void HandleUserSelectCharacter(ushort fromClientId, Message message)
    {
        string character = message.GetString();
        Console.WriteLine($"User ({fromClientId}) Selected Character ({character})");
        SendStringMessage(fromClientId, character, ServerToClientId.remoteUserSelectedCharacter, MessageSendMode.reliable);
    }

    // Helper Function
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
}
