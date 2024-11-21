using CasinoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibraryTests
{

    [TestClass]
    public class Extension_GamePacketTests
    {
        [TestMethod]
        public void Serialize_Deserialize_Success()
        {
            // Arrange
            ushort sourcePort = 1234;
            ushort destinationPort = 5678;
            byte packetType = 1;
            byte[] dataPayload = Encoding.UTF8.GetBytes("Test payload");

            // Create a new CasinoPacket
            var originalPacket = new CasinoPacket(sourcePort, destinationPort, packetType, dataPayload);

            // Act
            byte[] serializedData = originalPacket.Serialize();
            var deserializedPacket = CasinoPacket.Deserialize(serializedData);

            // Assert
            Assert.IsNotNull(deserializedPacket);
            Assert.AreEqual(originalPacket.SourcePort, deserializedPacket.SourcePort);
            Assert.AreEqual(originalPacket.DestinationPort, deserializedPacket.DestinationPort);
            Assert.AreEqual(originalPacket.PacketType, deserializedPacket.PacketType);
            Assert.AreEqual(originalPacket.PacketLength, deserializedPacket.PacketLength);
            Assert.AreEqual(originalPacket.UniquePacketId, deserializedPacket.UniquePacketId);
            Assert.AreEqual(originalPacket.ProtocolVersion, deserializedPacket.ProtocolVersion);
            Assert.AreEqual(originalPacket.CompressionFlag, deserializedPacket.CompressionFlag);
            Assert.AreEqual(originalPacket.SecureAuthenticationToken, deserializedPacket.SecureAuthenticationToken);
            Assert.AreEqual(originalPacket.TimestampLength, deserializedPacket.TimestampLength);
            Assert.AreEqual(originalPacket.DataPayload, deserializedPacket.DataPayload);


        }

        [TestMethod]
        public void PacketLength_Calculation_Success()
        {
            // Arrange
            ushort sourcePort = 1234;
            ushort destinationPort = 5678;
            byte packetType = 1;
            byte[] dataPayload = Encoding.UTF8.GetBytes("Test payload");

            // Create a new CasinoPacket
            var packet = new CasinoPacket(sourcePort, destinationPort, packetType, dataPayload);

            // Act
            uint expectedPacketLength = (uint)(47 + Encoding.UTF8.GetBytes(DateTime.Now.ToString()).Length + dataPayload.Length);

            // Assert
            Assert.AreEqual(expectedPacketLength, packet.PacketLength);
        }

        [TestMethod]
        public void GenerateSecureKey_Success()
        {
            // Arrange
            int keySize = 32;

            // Act
            byte[] secureKey = CasinoPacket.GenerateSecureKey(keySize);

            // Assert
            Assert.IsNotNull(secureKey);
            Assert.AreEqual(keySize, secureKey.Length);
        }

        [TestMethod]
        public void CombineWith_TwoArrays_Success()
        {
            // Arrange
            byte[] array1 = new byte[] { 0x01, 0x02, 0x03 };
            byte[] array2 = new byte[] { 0x04, 0x05 };

            // Act
            byte[] combined = array1.CombineWith(array2);

            // Assert
            byte[] expected = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            CollectionAssert.AreEqual(expected, combined);
        }

        [TestMethod]
        public void CombineWith_MultipleArrays_Success()
        {
            // Arrange
            byte[] array1 = new byte[] { 0x01, 0x02 };
            byte[] array2 = new byte[] { 0x03 };
            byte[] array3 = new byte[] { 0x04, 0x05, 0x06 };

            // Act
            byte[] combined = array1.CombineWith(array2, array3);

            // Assert
            byte[] expected = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            CollectionAssert.AreEqual(expected, combined);
        }

        [TestMethod]
        public void CombineWith_EmptyArrays_Success()
        {
            // Arrange
            byte[] array1 = new byte[] { };
            byte[] array2 = new byte[] { };

            // Act
            byte[] combined = array1.CombineWith(array2);

            // Assert
            byte[] expected = new byte[] { };
            CollectionAssert.AreEqual(expected, combined);
        }


        [TestMethod]
        public void CombineWith_NullArray_Success()
        {
            // Arrange
            byte[] array1 = new byte[] { 0x01, 0x02 };

            // Act
            byte[] combined = array1.CombineWith(null);

            // Assert
            //byte[] expected = new byte[] { 0x01, 0x02 };
            Assert.AreEqual(array1, combined);
        }
    }
}


    