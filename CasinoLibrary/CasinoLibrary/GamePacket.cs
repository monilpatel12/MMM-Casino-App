using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace CasinoLibrary
{
    [Serializable]
    public class CasinoPacket
    {
        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }
        public byte[] Timestamp { get; set; }
        public byte PacketType { get; set; }
        public uint PacketLength { get; set; }
        public uint UniquePacketId { get; set; }
        public byte ProtocolVersion { get; set; } = 1; // Default protocol version
        public byte CompressionFlag { get; set; } = 0; // 0 = no compression, 1 = compressed
        public byte[] SecureAuthenticationToken { get; set; }
        public byte[] DataPayload { get; set; }
        public byte TimestampLength { get; private set; }

        private static byte[] secureKey = GenerateSecureKey();

        public CasinoPacket(ushort sourcePort, ushort destinationPort, byte packetType, byte[] dataPayload)
        {   
            SourcePort = sourcePort;
            DestinationPort = destinationPort;
            PacketType = packetType;
            DataPayload = dataPayload;
            Timestamp = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
            TimestampLength = (byte)(Timestamp.Length);
            UniquePacketId = GenerateUniquePacketId();
            SecureAuthenticationToken = GenerateSecureAuthToken();

            PacketLength = (uint)(47 + Timestamp.Length + dataPayload.Length);
        }

        public static byte[] GenerateSecureKey(int keySize = 32)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[keySize];
                rng.GetBytes(randomBytes);
                return randomBytes;
            }
        }

        private uint GenerateUniquePacketId()
        {
            // Simple unique ID generation based on timestamp and source/dest ports
            var hash = BitConverter.GetBytes(PacketLength).CombineWith(BitConverter.GetBytes(SourcePort), Timestamp);
            var uniqueId = BitConverter.ToUInt32(hash, 0);
            return uniqueId;
        }

        private byte[] GenerateSecureAuthToken()
        {
            // The message should contain data that you want to be authenticated
            // In this case, we'll just use the timestamp and the packet's unique ID
            var message = Timestamp.CombineWith(BitConverter.GetBytes(UniquePacketId));

            using (var hmac = new HMACSHA256(secureKey))  // Use the secureKey directly
            {
                return hmac.ComputeHash(message);
            }
        }

        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            {
                // Convert each property to bytes and write to the MemoryStream
                memoryStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)SourcePort)), 0, sizeof(short));
                memoryStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)DestinationPort)), 0, sizeof(short));
                memoryStream.Write(new byte[] { TimestampLength }, 0, sizeof(byte));
                memoryStream.Write((Timestamp), 0, Timestamp.Length);
                memoryStream.Write(new byte[] { PacketType }, 0, sizeof(byte));
                memoryStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)PacketLength)), 0, sizeof(int));
                memoryStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)UniquePacketId)), 0, sizeof(int));
                memoryStream.Write(new byte[] { ProtocolVersion }, 0, sizeof(byte));
                memoryStream.Write(new byte[] { CompressionFlag }, 0, sizeof(byte));
                memoryStream.Write(SecureAuthenticationToken, 0, SecureAuthenticationToken.Length);
                memoryStream.Write(DataPayload, 0, DataPayload.Length);

                byte[] serializedData = memoryStream.ToArray();

                return serializedData;
            }
        }

        public static CasinoPacket Deserialize(byte[] data)
        {
            if (data == null || data.Length < 20) // Minimum length check based on your header size
                throw new ArgumentException("Data is null or does not meet minimum packet size.");

            using (var memoryStream = new MemoryStream(data))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                // Read each property from the MemoryStream
                var sourcePort = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                var destinationPort = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                var timestampLength = binaryReader.ReadByte();
                var timestamp = binaryReader.ReadBytes(timestampLength);
                var packetType = binaryReader.ReadByte();
                var packetLengthNetworkOrder = binaryReader.ReadInt32();
                var packetLength = (uint)IPAddress.NetworkToHostOrder(packetLengthNetworkOrder);

                if (packetLength > data.Length)
                    throw new InvalidOperationException($"Invalid packet length: {packetLength}");

                var uniquePacketId = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                var protocolVersion = binaryReader.ReadByte();
                var compressionFlag = binaryReader.ReadByte();
                var secureAuthToken = binaryReader.ReadBytes(32);
                var dataPayloadLength = (int)packetLength - (47 + timestampLength);

                if (dataPayloadLength < 0 || dataPayloadLength > memoryStream.Length - memoryStream.Position)
                    throw new InvalidOperationException($"Invalid data payload length: {dataPayloadLength}");

                var dataPayload = binaryReader.ReadBytes(dataPayloadLength);

                // Construct a new CasinoPacket with the deserialized data
                var packet = new CasinoPacket((ushort)sourcePort, (ushort)destinationPort, packetType, dataPayload)
                {
                    Timestamp = timestamp,
                    UniquePacketId = uniquePacketId,
                    ProtocolVersion = protocolVersion,
                    CompressionFlag = compressionFlag,
                    SecureAuthenticationToken = secureAuthToken
                };
                return packet;
            }
        }

        public void setPacket(ushort sourcePort, ushort destinationPort, byte packetType, byte[] dataPayload)
        {
            SourcePort = sourcePort;
            DestinationPort = destinationPort;
            PacketType = packetType;
            DataPayload = dataPayload;
            Timestamp = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
            UniquePacketId = GenerateUniquePacketId();
            SecureAuthenticationToken = GenerateSecureAuthToken();
            PacketLength = (uint)(47 + Timestamp.Length + dataPayload.Length);
            TimestampLength = (byte)Timestamp.Length;
        }
    }
}
