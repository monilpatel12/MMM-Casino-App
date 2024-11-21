using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

namespace CasinoLibrary
{
    public class TCPServer
    {
        public TcpListener server;
        public TcpClient client;
        
        public NetworkStream stream;
        public byte[] RxBytes;
        public byte[] TxBytes;
        int bytesRead;

        public CasinoPacket packet;
        public string dataPayloadString;

        string loggingPath;
        public bool connection;

        public TCPServer()
        {
            InitializeNonNetworkResources();
            SetupNetwork();
        }

        protected TCPServer(bool setupNetwork)
        {
            if (setupNetwork)
            {
                SetupNetwork();
            }
            InitializeNonNetworkResources();
        }

        protected void InitializeNonNetworkResources()
        {
            // Setup buffers and any other non-network resources here
            RxBytes = new byte[131072];
            TxBytes = new byte[131072];
            dataPayloadString = "";
            byte[] dataPayload = Encoding.UTF8.GetBytes("Mock");
            packet = new CasinoPacket(27000, 27000, 100, dataPayload);
            loggingPath = "../../../ServerLogs.txt";
        }

        private void SetupNetwork()
        {
            //Setup Server
            server = new TcpListener(IPAddress.Any, 27000);
            server.Start();
            Console.WriteLine("Server started. Waiting for a connection...");

            //Setup Client
            client = server.AcceptTcpClient();
            connection = true;
            Console.WriteLine("Connected!");

            //Setup stream
            stream = client.GetStream();

            //Setup logging file
            File.WriteAllText(loggingPath, String.Empty);

            //Receive the initial hello packet
            packet = receivePacket();
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

        public virtual CasinoPacket receivePacket()
        {
            try
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
            catch 
            {
                packet.setPacket(27000, 27000, 0, [0]);
                return packet;
            }
        }

        public virtual CasinoPacket receiveImagePacket()
        {
            // It's assumed that the caller knows that the next packet will be an image.
            bytesRead = stream.Read(RxBytes, 0, RxBytes.Length);

            // Here, we are not converting the data to a string since it's binary data for the image
            packet = CasinoPacket.Deserialize(RxBytes[..bytesRead]);

            // Log receipt of the packet but don't attempt to process it as a string
            log(1, "Received image packet");

            return packet;
        }


        public virtual CasinoPacket sendPacket(byte type, string message)
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

        public virtual CasinoPacket sendImagePacket(byte type, string imagePath)
        {
            // Read the image into a byte array
            byte[] dataPayload = File.ReadAllBytes(imagePath);

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
            // Close everything
            client.Close();
            server.Stop();

            connection = false;
        }

        public void runProtocol(PlayerInfo p)
        {
            switch (packet.PacketType) 
            {
                //Client has disconnected
                case 0:
                    Console.WriteLine("Client disconected, terminating server...\n");

                    shutDown();
                    break;
                case 1:
                    Console.WriteLine("Start BlackJack Game request received. Initializing a new game...\n");

                    string[] data = dataPayloadString.Split(',');
                    p.bet = Int32.Parse(data[0]);
                    p.balance = Int32.Parse(data[1]);
                    
                    BlackjackGame BJG = new BlackjackGame(this, p);
                    BJG.StartGame();
                    break;
                case 2:
                    Console.WriteLine("Player has joined the roulette table. Initializing a new game...\n");

                    RouletteGame RG = new RouletteGame(this, p);
                    RG.listen();
                    break;
                case 3:
                    Console.WriteLine("Request to change profile picture, saving image...\n");
                    packet = receiveImagePacket();
                    SaveImage(packet.DataPayload);
                    break;
                case 4:
                    Console.WriteLine("Request for profile picture, sending image...\n");
                    packet = sendImagePacket(0, "../../../Saved Images/ProfilePic.jpg");
                    break;
                case 5:
                    Console.WriteLine("Login request received, fetching results...\n");
                    checkLogin();
                    break;
                case 6:
                    Console.WriteLine("Register request received, attempting to create a new account...\n");
                    createAccount();
                    break;
            }
        }

        private void createAccount()
        {
            var parts = dataPayloadString.Split(',');

            string username = parts[0].Trim();
            string password = parts[1].Trim();
            string filePath = @"../../../UserAuth.txt";

            // Read all lines from the file.
            string[] lines = System.IO.File.ReadAllLines(filePath);

            // Check if username already exists.
            foreach (string line in lines)
            {
                var lineParts = line.Split(',');
                if (lineParts[0].Trim() == username)
                {
                    packet = sendPacket(1, "Username is already taken"); // Username already exists
                    return;
                }
            }

            // Username doesn't exist, so create the account.
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine($"{username}, {password}");
            }

            packet = sendPacket(0, "Register successful"); // Successful register
            return;
        }

        private void SaveImage(byte[] imageData)
        {
            // Determine the path where you want to save the image
            string imagePath = "../../../Saved Images/ProfilePic.jpg";

            // Write the binary data to a file
            File.WriteAllBytes(imagePath, imageData);
            Console.WriteLine($"Image saved to {imagePath}");
        }

        private void checkLogin() 
        {
            var parts = dataPayloadString.Split(',');

            string username = parts[0];
            string password = parts[1];

            string[] lines = System.IO.File.ReadAllLines(@"../../../UserAuth.txt");

            foreach (string line in lines)
            {
                var lineParts = line.Split(',');
                if (lineParts.Length != 2) continue; // Skip invalid lines

                string fileUsername = lineParts[0].Trim();
                string filePassword = lineParts[1].Trim();

                if (fileUsername == username)
                {
                    if (filePassword == password)
                    {
                        packet = sendPacket(0, "Login successful"); // Valid username and password
                        return;
                    }
                    else
                    {
                        packet = sendPacket(2, "Invalid password"); // Valid username but invalid password
                        return;
                    }
                }
            }

            packet = sendPacket(1, "Invalid username"); // Username not found
            return;
        }
    }
}
