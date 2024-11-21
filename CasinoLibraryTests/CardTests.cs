using CasinoLibrary;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Sockets;

namespace CasinoLibraryTests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        [Description("Verifies that a Card object is created with specified suit, rank, and value parameters.")]
        public void TestCardCreationWithValidParameters()
        {
            // Arrange
            string suit = "Hearts";
            string rank = "10";
            int value = 10;

            // Act
            Card card = new Card(suit, rank, value);

            // Assert
            Assert.AreEqual(suit, card.Suit);
            Assert.AreEqual(rank, card.Rank);
            Assert.AreEqual(value, card.Value);
        }

        [TestMethod]
        [Description("Ensures that the properties Suit, Rank, and Value are immutable after the Card object has been created.")]
        public void TestCardPropertyImmutability()
        {
            // Arrange
            Card card = new Card("Hearts", "10", 10);

            // Act & Assert
            // You'll need to assert that the properties cannot be changed. This could be 
            // done by trying to set the properties and catching any expected exceptions,
            // or by using reflection to assert that set accessors are private.
        }
    }
}