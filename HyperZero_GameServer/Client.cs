using System.Net;
using System.Net.Sockets;
using System;

namespace UnityTestGameServer
{

   
    public class Client
    {
        public static int DATA_BUFFER_SIZE = 4096;
        public int id;
        public TCP tcp;
        public UDP udp;

        public Client(int id)
        {
            this.id = id;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

    }

    public class TCP
    {
        public TcpClient clientConn;
        private readonly int id;
        private NetworkStream stream;
        private byte[] recieveBuffer;
        private Packet recievedData;

        public TCP(int id)
        {
            this.id = id;
        }

        public void Connect(TcpClient clientConn)
        {
            this.clientConn = clientConn;
            clientConn.ReceiveBufferSize = Client.DATA_BUFFER_SIZE;
            clientConn.SendBufferSize = Client.DATA_BUFFER_SIZE;
            recievedData = new Packet();

            stream = clientConn.GetStream();
            recieveBuffer = new byte[clientConn.ReceiveBufferSize];
            stream.BeginRead(recieveBuffer, 0, clientConn.ReceiveBufferSize, RecieveCallback, null);

            ServerSend.Welcome(id, "WELCOME TO THE SERVER FUCKNUTS");
        }

        public void RecieveCallback(IAsyncResult result)
        {
            try
            {
                int byteLen = stream.EndRead(result);
                if (byteLen <= 0) return;

                byte[] data = new byte[byteLen];
                Array.Copy(recieveBuffer, data, byteLen);

                stream.BeginRead(recieveBuffer, 0, clientConn.ReceiveBufferSize, RecieveCallback, null);
                recievedData.Reset(HandleData(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong upon reading data buffer");
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                if(clientConn != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }

            } catch(Exception ex)
            {
                Console.WriteLine("Failed to send packet");
                Console.WriteLine(ex);
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLen = 0;
            recievedData.SetBytes(data);

            // first 4 bytes is always the len of the packet!
            if (recievedData.UnreadLength() >= 4)
            {
                packetLen = recievedData.ReadInt();
                if (recievedData.UnreadLength() < 1) return true; // will reset the packet
            }


            while (packetLen > 0 && packetLen <= recievedData.UnreadLength())
            {
                byte[] packetBytes = recievedData.ReadBytes(packetLen);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        // packet Id correspond to a specific id of a function that defines how to handle this type of packet
                        int packetId = packet.ReadInt();
                        Server.packetHandlers[packetId](id, packet);
                    }

                });
                packetLen = 0;

                // todo -> duplicate code -> cleanup
                if (recievedData.UnreadLength() >= 4)
                {
                    packetLen = recievedData.ReadInt();
                    if (recievedData.UnreadLength() < 1) return true; // will reset the packet
                }
            }

            return packetLen <= 1;
        }




    }
    public class UDP
    {
        public IPEndPoint endPoint;
        private int id;

        public UDP(int id)
        {
            this.id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            ServerSend.UDPTest(id);
        }

        public void SendData(Packet packet)
        {
            Server.SendUDPData(endPoint, packet);
        }

        public void HandleData(Packet packet)
        {
            int packetLen = packet.ReadInt(); // failed to read packet length
            byte[] data = packet.ReadBytes(packetLen);
            

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetHandlerId = packet.ReadInt();
                    Server.packetHandlers[packetHandlerId](id, packet);
                }
            });
        }


    }
}
