using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibrary
{
    public class TCPClient
    {
        TcpClient client;

        NetworkStream stream;
        byte[] RxBytes;
        byte[] TxBytes;
        int bytesRead;

        public CasinoPacket packet;
        public string dataPayloadString;
        byte[] dataPayload;

        string loggingPath;

        public TCPClient()
        {
            //Create a TcpClient
            client = new TcpClient("127.0.0.1", 27000);

            //Setup stream and buffers
            stream = client.GetStream();
            RxBytes = new byte[131072];
            bytesRead = 0;
            dataPayloadString = "";

            //Setup logging file
            loggingPath = "../../../ClientLogs.txt";
            File.WriteAllText(loggingPath, String.Empty);

            //Setup the initial hello packet
            dataPayload = Encoding.UTF8.GetBytes("Hello, Casino Server!");
            packet = new CasinoPacket(27000, 27000, 0, dataPayload);

            //Serialize the packet then send
            TxBytes = packet.Serialize();
            stream.Write(TxBytes, 0, TxBytes.Length);
        }

        private void log(int imageFlag, string transmissionType)
        {
            string headerInfo = $"{transmissionType}: SourcePort={packet.SourcePort}, DestinationPort={packet.DestinationPort}, Timestamp={DateTime.Parse(Encoding.UTF8.GetString(packet.Timestamp))}, PacketType={packet.PacketType}, " +
                                $"\nPacketLength={packet.PacketLength}, UniquePacketID={packet.UniquePacketId}, ProtocolVersion={packet.ProtocolVersion}, CompressionFlag={packet.CompressionFlag}{Environment.NewLine}";

            File.AppendAllText(loggingPath, headerInfo + Environment.NewLine);

            if (imageFlag == 0)
            {
                File.AppendAllText(loggingPath, $"DataPayload: {dataPayloadString}{Environment.NewLine}{Environment.NewLine}");
                Console.WriteLine($"DataPayload: {dataPayloadString}\n");
            }

            Console.WriteLine(headerInfo);
        }

        public CasinoPacket receivePacket()
        {
            bytesRead = stream.Read(RxBytes, 0, RxBytes.Length);

            //Setup packet
            packet = CasinoPacket.Deserialize(RxBytes[..bytesRead]);

            // Process packet
            dataPayloadString = Encoding.UTF8.GetString(packet.DataPayload);

            //Log packet
            log(0, "Received packet");

            return packet;
        }

        public CasinoPacket receiveImagePacket()
        {
            // It's assumed that the caller knows that the next packet will be an image.
            bytesRead = stream.Read(RxBytes, 0, RxBytes.Length);

            // Here, we are not converting the data to a string since it's binary data for the image
            packet = CasinoPacket.Deserialize(RxBytes[..bytesRead]);

            // Log receipt of the packet but don't attempt to process it as a string
            log(1, "Received image packet");

            return packet;
        }

        public CasinoPacket sendPacket(byte type, string message)
        {
            //Setup TxBytes
            byte[] dataPayload = Encoding.UTF8.GetBytes(message);

            //Setup packet
            packet.setPacket(27000, 27000, type, dataPayload);

            // Serialize and send the packet
            TxBytes = packet.Serialize();
            stream.Write(TxBytes, 0, TxBytes.Length);

            //Log the packet sent
            log(0, "Sent packet");

            return packet;
        }

        public CasinoPacket sendImagePacket(byte type, byte[] dataPayload)
        {
            //Setup packet
            packet.setPacket(27000, 27000, type, dataPayload);

            // Serialize and send the packet
            TxBytes = packet.Serialize();
            stream.Write(TxBytes, 0, TxBytes.Length);

            //Log the packet sent
            log(1, "Sent image packet");

            return packet;
        }

        public void shutDown()
        {
            client.Close();
        }
    }
}
