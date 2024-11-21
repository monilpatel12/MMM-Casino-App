using CasinoLibrary;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CasinoLibraryTests
{
    [TestClass]
    public class PlayerInfoTests
    {
        [TestMethod]
        public void PlayerInfo_DefaultConstructor()
        {
            // Arrange
            var playerInfo = new PlayerInfo();

            // Assert
            Assert.AreEqual(0, playerInfo.bet);
            Assert.AreEqual(1000, playerInfo.balance);
            Assert.AreEqual(0, playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_BlackjackPushOrWin()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(0, 0); // Blackjack push
            double expectedBalance = 1000 + (100 * 1.5) + (100 * 2);

            // Assert
            Assert.AreEqual(expectedBalance, playerInfo.balance);
            Assert.AreEqual((100 * 1.5) + (100 * 2), playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_BlackjackWin()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(0, 1); // Blackjack win

            // Assert
            Assert.AreEqual(1200, playerInfo.balance);
            Assert.AreEqual(100 * 2, playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_BlackjackLose()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(0, 2); // Blackjack lose

            // Assert
            Assert.AreEqual(1000, playerInfo.balance);
            Assert.AreEqual(0, playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_BlackjackPush()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(0, 3); // Blackjack push

            // Assert
            Assert.AreEqual(1100, playerInfo.balance);
            Assert.AreEqual(100, playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_BlackjackBust()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(0, 4); // Blackjack bust

            // Assert
            Assert.AreEqual(1000, playerInfo.balance);
            Assert.AreEqual(0, playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_Roulette1to1()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(1, 0); // Roulette 1 to 1

            // Assert
            Assert.AreEqual(1100, playerInfo.balance);
            Assert.AreEqual(200, playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_RouletteNumber()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(1, 1); // Roulette number

            // Assert
            Assert.AreEqual(4500, playerInfo.balance);
            Assert.AreEqual(3600, playerInfo.winnings);
        }

        [TestMethod]
        public void PlayerInfo_CalculatePayout_RouletteLose()
        {
            // Arrange
            var playerInfo = new PlayerInfo();
            playerInfo.bet = 100;

            // Act
            playerInfo.calculatePayout(1, 2); // Roulette lose

            // Assert
            Assert.AreEqual(900, playerInfo.balance);
            Assert.AreEqual(0, playerInfo.winnings);
        }
    }
}
