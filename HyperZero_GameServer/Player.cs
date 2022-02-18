using System;
using System.Numerics;

namespace HyperZero_GameServer
{
    class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        public Player(int id, string username, Vector3 spawnPosition)
        {
            this.id = id;
            this.username = username;
            position = spawnPosition;
            rotation = Quaternion.Identity;
        }
    }
}
