using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HyperZero_GameServer
{
    
    class ServerSend
    {

        private static void SendTCP(int clientId, Packet packet)
        {
            packet.WriteLength();
            Server.players[clientId].tcp.SendData(packet);
        }

        private static void SendUDP(int clientId, Packet packet)
        {
            packet.WriteLength();
            Server.players[clientId].udp.SendData(packet);
        }

        private static void SendTCPAllClients(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            foreach (int id in Server.players.Keys)
            {
                if(id != exceptClient) Server.players[id].tcp.SendData(packet);
            }
        }

        private static void SendUDPAllClients(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            foreach (int id in Server.players.Keys)
            {
                if (id != exceptClient) Server.players[id].udp.SendData(packet);
            }
        }

        private static void SendUDPAllClients(Packet packet)
        {
            packet.WriteLength();
            foreach (int id in Server.players.Keys)
            {
                Server.players[id].udp.SendData(packet);
            }
        }

        #region Messages

        public static void Welcome(int clientId, string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(msg);
                packet.Write(clientId);

                SendTCP(clientId, packet);
            }
        }

        /// <summary>Send info about where a player should spawn (TCP)</summary>
        public static void SpawnPlayer(int clientId, Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                packet.Write(player.id);
                packet.Write(player.username);
                packet.Write(player.position);
                packet.Write(player.rotation);

                SendTCP(clientId, packet);
                // Console.WriteLine($"Sending Player Spawn Request for player {player.username} to client {clientId}");

            }
        }

        public static void PlayerPos(Player player)
        {
            using(Packet packet = new Packet((int)ServerPackets.playerPosition))
            {
                packet.Write(player.id);
                packet.Write(player.position);
                ServerSend.SendUDPAllClients(packet);
            }
        }



        public static void PlayerRotation(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerRotation))
            {
                packet.Write(player.id);
                packet.Write(player.rotation);
                ServerSend.SendUDPAllClients(player.id, packet);
            }
        }

        #endregion


    }
}
