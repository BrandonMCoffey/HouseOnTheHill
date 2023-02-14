using Riptide;

namespace Betrayal.ConsoleServer
{
    // Messages sent from the client (Local) to the server (Remote)
    internal enum ClientToServerId : ushort
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
    internal enum ServerToClientId : ushort
    {
        createRemoteUser = 1, // reliable (ushort client, string userName)
        remoteUserSelectCharacter, // reliable (ushort client, int characterIndex)
        remoteUserReadyUp, // reliable (ushort client, bool ready)

        lobbyTimerCountdown, // reliable (bool countdown, float countdownTime)
        loadGameScene, // reliable
        setupGame, // reliable (ushort[] turnOrder)

        updateRemoteUserTraits, // reliable (ushort client, int[4] traitValues)
        updateRemoteUserItemsHeld, // reliable (ushort client, int[] itemIds)
        updateRemoteUserTransform, // unreliable (ushort client, int roomId, float[6] TransformData)

        receiveRoomCreated, // reliable (ushort client, int roomId, int floor, int x, int y, int rotation)
        receiveAnnouncement, // reliable (ushort client, string title, string text)

        updateCurrentPlayerTurn, // reliable (ushort usersTurn)
    }

    internal class ProgramMessageHandler
    {
        public static void PlayerJoinedServer(ushort clientId)
        {
            foreach (var pair in Program.PlayerData)
            {
                var id = pair.Key;
                if (id == clientId)
                    continue;
                var nameMessage = CreateMessage(id, ServerToClientId.createRemoteUser);
                nameMessage.AddString(Program.PlayerData[id].UserName);
                Program.SendMessageToClient(nameMessage, clientId);

                var characterMessage = CreateMessage(id, ServerToClientId.remoteUserSelectCharacter);
                characterMessage.AddInt(Program.PlayerData[id].Character);
                Program.SendMessageToClient(characterMessage, clientId);

                var readyMessage = CreateMessage(id, ServerToClientId.remoteUserReadyUp);
                readyMessage.AddBool(Program.PlayerData[id].Ready);
                Program.SendMessageToClient(readyMessage, clientId);
            }

            Message CreateMessage(ushort id, ServerToClientId messageType)
            {
                Message message = Message.Create(MessageSendMode.reliable, messageType);
                message.AddUShort(id);
                return message;
            }
        }

        [MessageHandler((ushort)ClientToServerId.clientConnectedToServer)]
        private static void HandleClientConnectedToServer(ushort fromClientId, Message message)
        {
            string name = message.GetString();

            Program.PlayerData[fromClientId].UserName = name;
            ProgramMessageHelper.SendStringMessage(fromClientId, name, ServerToClientId.createRemoteUser, MessageSendMode.reliable);

            PrintUserEvent(fromClientId, "Joined the Lobby");
        }

        [MessageHandler((ushort)ClientToServerId.localUserSelectCharacter)]
        private static void HandleLocalUserSelectCharacter(ushort fromClientId, Message message)
        {
            int character = message.GetInt();

            Program.PlayerData[fromClientId].Character = character;
            ProgramMessageHelper.SendIntMessage(fromClientId, character, ServerToClientId.remoteUserSelectCharacter, MessageSendMode.reliable);

            PrintUserEvent(fromClientId, $"Selected Character ({character})");
        }

        [MessageHandler((ushort)ClientToServerId.localUserReadyUp)]
        private static void HandleLocalUserReadyUp(ushort fromClientId, Message message)
        {
            bool ready = message.GetBool();

            Program.PlayerData[fromClientId].Ready = ready;
            Program.CheckAllPlayersReady();
            ProgramMessageHelper.SendBoolMessage(fromClientId, ready, ServerToClientId.remoteUserReadyUp, MessageSendMode.reliable);

            PrintUserEvent(fromClientId, $"is now ({(ready ? "Ready" : "Not Ready")})");
        }

        [MessageHandler((ushort)ClientToServerId.gameLoaded)]
        private static void HandleLocalUserGameLoaded(ushort fromClientId, Message message)
        {
            if (Program.GameStarted)
            {
                Message sendMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.setupGame);
                sendMessage.AddUShorts(Program.TurnOrder.ToArray());
                Program.SendMessageToClient(sendMessage, fromClientId);

                foreach (var room in Program.Rooms)
                {
                    var roomMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.receiveRoomCreated);
                    roomMessage.AddUShort(room.Client);
                    roomMessage.AddInt(room.Id);
                    roomMessage.AddInt(room.Floor);
                    roomMessage.AddInt(room.X);
                    roomMessage.AddInt(room.Z);
                    roomMessage.AddInt(room.Rot);
                    Program.SendMessageToClient(roomMessage, fromClientId);
                }
            }
            else
            {
                Program.PlayerData[fromClientId].GameLoaded = true;
                Program.CheckAllPlayersLoaded();
            }
        }

        [MessageHandler((ushort)ClientToServerId.updateLocalUserTransform)]
        private static void HandleUpdateLocalUserTransform(ushort fromClientId, Message message)
        {
            var data = message.GetFloats(6);
            ProgramMessageHelper.SendFloatArrayMessage(fromClientId, data, ServerToClientId.updateRemoteUserTransform, MessageSendMode.unreliable);
        }

        [MessageHandler((ushort)ClientToServerId.localUserEndTurn)]
        private static void HandleLocalUserEndTurn(ushort fromClientId, Message message)
        {
            var playerTurn = Program.IncrementTurnOrder();
            var sendMessage = Message.Create(MessageSendMode.reliable, ServerToClientId.updateCurrentPlayerTurn);
            sendMessage.AddUShort(playerTurn);
            Program.SendMessageToAll(sendMessage);

            PrintUserEvent(fromClientId, "Ended Their Turn");
            PrintUserEvent(playerTurn, "Is now the Active Player");
        }

        [MessageHandler((ushort)ClientToServerId.createNewRoom)]
        private static void HandleCreateNewRoom(ushort fromClientId, Message message)
        {
            var data = message.GetInts(5); // roomId, floor, x, z, rot

            Program.Rooms.Add(new RoomData(fromClientId, data));
            ProgramMessageHelper.SendIntArrayMessage(fromClientId, data, ServerToClientId.receiveRoomCreated, MessageSendMode.reliable);

            PrintUserEvent(fromClientId, $"New Room Created ({data[0]})");
        }

        public static void PrintUserEvent(ushort user, string message)
        {
            Program.Print($"({user}) {Program.PlayerData[user].UserName} {message}");
        }
    }
}
