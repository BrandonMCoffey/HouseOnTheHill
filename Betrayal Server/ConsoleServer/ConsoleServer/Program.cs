using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Betrayal.ConsoleServer
{
    internal class Program
    {
        #region Variables

        private static Server server;
        private static bool isRunning;

        private const ushort Timeout = 10000; // 10s
        private const ushort Port = 7777;

        private const float LobbyCountdown = 5;

        private static List<ushort> connectedClients;
        public static Dictionary<ushort, PlayerData> PlayerData;
        private static List<PlayerData> leftoverPlayerData;

        private static GameState gameState = GameState.lobby;

        private static float lobbyCountdownTimer;
        public static List<ushort> TurnOrder;
        public static int TurnOrderIndex;

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
            PlayerData = new Dictionary<ushort, PlayerData>();
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
            PlayerData.Add(clientId, new PlayerData());
            CheckAllPlayersReady();

            ProgramMessageHandler.PlayerJoinedServer(clientId);

            Console.WriteLine($"Client connected: ({clientId})");
        }

        private static void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            var clientId = e.Id;
            if (GameStarted) leftoverPlayerData.Add(PlayerData[clientId]);
            connectedClients.Remove(clientId);
            PlayerData.Remove(clientId);
            CheckAllPlayersReady();

            Console.WriteLine($"Client disconnected ({e.Id})");
        }

        #endregion

        public static void CheckAllPlayersReady()
        {
            if (GameStarted)
                return;
            int ready = PlayerData.Sum(pair => pair.Value.Ready ? 1 : 0);
            int total = PlayerData.Count;
            var allReady = ready == total;

            gameState = allReady ? GameState.lobbyAllReady : GameState.lobby;
            lobbyCountdownTimer = LobbyCountdown;

            Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.lobbyTimerCountdown);
            message.AddBool(allReady);
            message.AddFloat(LobbyCountdown);
            SendMessageToAll(message);

            Console.WriteLine(allReady ? $"All Players Ready! Starting Game in {LobbyCountdown} seconds." : $"Players Ready ({ready}/{total})");
        }

        public static void CheckAllPlayersLoaded()
        {
            bool allLoaded = PlayerData.Aggregate(true, (current, pair) => current & pair.Value.GameLoaded);
            if (allLoaded)
            {
                gameState = GameState.playingGame;
                TurnOrderIndex = 0;
                TurnOrder = new List<ushort>();
                foreach ((ushort clientId, PlayerData playerData) in PlayerData)
                {
                    if (playerData.Character > 0)
                        TurnOrder.Add(clientId);
                }

                Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.setupGame);
                message.AddUShorts(TurnOrder.ToArray());
                SendMessageToAll(message);

                Console.WriteLine("All Players Loaded. Setting Up Game!");
            }
        }

        public static ushort IncrementTurnOrder()
        {
            TurnOrderIndex++;
            if (TurnOrderIndex >= TurnOrder.Count) TurnOrderIndex = 0;
            return TurnOrder[TurnOrderIndex];
        }

        #region Messages

        public static void SendMessageToClient(Message message, ushort toClientId)
        {
            server.Send(message, toClientId);
        }

        public static void SendMessageFromClient(Message message, ushort fromClientId)
        {
            foreach (ushort client in connectedClients.Where(client => client != fromClientId))
                server.Send(message, client);
        }

        public static void SendMessageToAll(Message message)
        {
            foreach (ushort client in connectedClients)
                server.Send(message, client);
        }

        #endregion

        public static void Print(string message) => Console.WriteLine(message);

        private static void PrintUserInfo()
        {
            foreach (var client in connectedClients)
            {
                Console.WriteLine(client);
                if (PlayerData.ContainsKey(client)) Console.WriteLine(PlayerData[client].PrintInfo());
            }
        }
    }
}
