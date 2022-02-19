using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace HyperZero_GameServer
{
    class Server
    {
        public delegate void PacketHandler(int id, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static int MaxPlayers {get; set;}
        public static int Port { get; set; }
        public static Dictionary<int, Client> players = new Dictionary<int, Client>();

        public static TcpListener tcpListener;
        public static UdpClient udpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            InitializeServerData();
            tcpListener = new TcpListener(IPAddress.Any, port);
            udpListener = new UdpClient(Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            udpListener.BeginReceive(OnUdpRecieved, null);
            Console.WriteLine($"Server started on port {port}");
        }

        public static void OnUdpRecieved(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                
                udpListener.BeginReceive(OnUdpRecieved, null);

                if (data.Length < 4) return;
                
                using (Packet packet = new Packet(data))
                {
                    int playerId = packet.ReadInt();

                    // if (clientId == 0) return;

                    if (players[playerId].udp.endPoint == null)
                    {
                        // client is registering to game
                        players[playerId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (players[playerId].udp.endPoint.Equals(clientEndPoint))
                    {
                        players[playerId].udp.HandleData(packet); 

                    } else
                    {
                        Console.WriteLine($"User at Address {clientEndPoint.ToString()} seems to be impersonating another player...");
                    }
                }

            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }


            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
            packetHandlers[(int)ClientPackets.playerMovement] = ServerHandle.PlayerMove;



            Console.WriteLine("Init packet handlers...");
        }

    }
}
