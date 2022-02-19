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

        private float moveSpd = 5f / Constants.TICKS_PER_SECOND; // applies fixedUpdate
        private bool[] inputs;

        public Player(int id, string username, Vector3 spawnPosition)
        {
            this.id = id;
            this.username = username;
            position = spawnPosition;
            rotation = Quaternion.Identity;
            inputs = new bool[4];
        }

        public void SetInputs(bool[] inputs, Quaternion rotation)
        {
            this.inputs = inputs;
            this.rotation = rotation;
        }

        public void Update()
        {
            Vector2 inputDir = Vector2.Zero;
            if (inputs[0]) inputDir.Y += 1; // W
            if (inputs[1]) inputDir.Y -= 1; // S
            if (inputs[2]) inputDir.X += 1; // A
            if (inputs[3]) inputDir.X -= 1; // D

            Move(inputDir);
        }

        public void Move(Vector2 direction)
        {
            Vector3 playerForwardDir = Vector3.Transform(new Vector3(0, 0, 1), rotation);
            Vector3 playerRightDir = Vector3.Normalize(Vector3.Cross(playerForwardDir, new Vector3(0, 1, 0)));

            Vector3 moveDirection = playerRightDir * direction.X + playerForwardDir *  direction.Y;
            position += moveDirection * moveSpd;

            ServerSend.PlayerPos(this);
            ServerSend.PlayerRotation(this);
        }
    }
}
