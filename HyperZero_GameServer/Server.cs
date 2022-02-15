using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace UnityTestGameServer
{
    class Server
    {
        public delegate void PacketHandler(int id, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static int MaxPlayers {get; set;}
        public static int Port { get; set; }
        public static Dictionary<int, Client> players = new Dictionary<int, Client>();

        public static TcpListener tcpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            InitializeServerData();
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Server started on port {port}");
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient clientConnection = tcpListener.EndAcceptTcpClient(result);
            Console.WriteLine($"Connecting Client at IP Address {clientConnection.Client.RemoteEndPoint}");
            
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            for (int i = 1; i < MaxPlayers; i++)
            {
                if (players[i].tcp.clientConn == null)
                {
                    players[i].tcp.Connect(clientConnection);
                    return;
                }
            }
            Console.WriteLine("Failed to connect player. Lobby is full.");
        }

        private static void InitializeServerData()
        {
            for(int i=1;i <= MaxPlayers; i++)
            {
                players[i] = new Client(i);
            }

            packetHandlers = new Dictionary<int, PacketHandler>();
            packetHandlers[(int)ClientPackets.welcomeReceived] = ServerHandle.WelcomeRecieved;
            Console.WriteLine("Init packet handlers...");
        }

    }
}
