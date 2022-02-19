using System;
using System.Numerics;

namespace HyperZero_GameServer
{
    class ServerHandle
    {
        public static void WelcomeRecieved(int clientId, Packet packet)
        {
            // send as an int then string
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"{Server.players[clientIdCheck].tcp.clientConn.Client.RemoteEndPoint} connected. Desired Username: {username}");

            if(clientId != clientIdCheck)
            {
                Console.WriteLine("Oh shit oh fuck, the client ID doesn't match the id sent in the packet...");
            }

            Server.players[clientId].SendIntoGame(username);
        }

        public static void OnUdpTestRecieved(int clientid, Packet packet)
        {
            string msg = packet.ReadString();
            Console.WriteLine(msg);
        }

        public static void PlayerMove(int clientId, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i=0;i<inputs.Length;i++)
            {
                inputs[i] = packet.ReadBool();
            }
            // CURRENTLY AUTHORITY FOR ROTATION LIES W CLIENT!!!
            Quaternion rotation = packet.ReadQuaternion(); 

            Server.players[clientId].playerRef.SetInputs(inputs, rotation);
        }
    }
}
