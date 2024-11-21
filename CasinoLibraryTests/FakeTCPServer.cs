using CasinoLibrary;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Text;

namespace CasinoLibraryTests
{
    public class MockTCPServer : TCPServer
    {
        private List<CasinoPacket> receivedPackets = new List<CasinoPacket>();
        private List<CasinoPacket> sentPackets = new List<CasinoPacket>();

        byte[] buffer = new byte[1024];

        public MockTCPServer() : base(setupNetwork: false)
        {
            receivedPackets = new List<CasinoPacket>();
            sentPackets = new List<CasinoPacket>();
        }

        public override CasinoPacket receivePacket()
        {
            // For example, return the next packet in a queue of expected packets.
            if (receivedPackets.Any())
            {
                var packet = receivedPackets[0];
                buffer = packet.Serialize();
                packet = CasinoPacket.Deserialize(buffer);
                dataPayloadString = Encoding.UTF8.GetString(packet.DataPayload);
                receivedPackets.RemoveAt(0);
                return packet;
            }

            // Default behavior or throw an exception if expected behavior is not set.
            throw new InvalidOperationException("No packets to receive");
        }

        public void SetupReceive(byte type, string v)
        {
            packet = new CasinoPacket(27000, 27000, type, Encoding.UTF8.GetBytes(v));
            receivedPackets.Add(packet);      
        }

        public override CasinoPacket sendPacket(byte type, string message)
        {
            // Convert the message string to a byte array for the data payload
            byte[] dataPayload = Encoding.UTF8.GetBytes(message);

            // Define source and destination ports for the packet
            ushort sourcePort = 27000; // Example source port, use the appropriate value
            ushort destinationPort = 27000; // Example destination port, use the appropriate value

            // Create a new packet with the provided type and the byte array of the message
            var packet = new CasinoPacket(sourcePort, destinationPort, type, dataPayload);

            // Add the newly created packet to the list of sent packets
            sentPackets.Add(packet);

            dataPayloadString = Encoding.UTF8.GetString(packet.DataPayload);

            return packet;
        }

        // Add methods to setup test scenarios, e.g., enqueue packets to be received.
        public void EnqueuePacketForReception(CasinoPacket packet)
        {
            receivedPackets.Add(packet);
        }

        // Add methods to inspect sent packets, if necessary.
        public List<CasinoPacket> GetSentPackets()
        {
            return sentPackets;
        }
    }
}