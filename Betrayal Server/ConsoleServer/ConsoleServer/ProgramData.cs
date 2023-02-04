using System.Collections.Generic;
using System.Numerics;

namespace Betrayal.ConsoleServer
{
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
}
