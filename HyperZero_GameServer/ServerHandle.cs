using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityTestGameServer
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
        }

        public static void OnUdpTestRecieved(int clientid, Packet packet)
        {
            string msg = packet.ReadString();
            Console.WriteLine(msg);
        }
    }
}
