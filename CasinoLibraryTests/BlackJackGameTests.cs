using CasinoLibrary;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Sockets;

namespace CasinoLibraryTests
{
    [TestClass]
    public class BlackjackGameTests
    {
        private BlackjackGame game;
        private PlayerInfo player;
        private MockTCPServer fakeServer;
        private int initialDeckSize = 52;

        [TestInitialize]
        public void Initialize()
        {
            // Initialize game with a fake server or mock server here
            fakeServer = new MockTCPServer();
            player = new PlayerInfo();
            game = new BlackjackGame(fakeServer, player);
        }

        [TestMethod]
        public void GameInitializesCorrectly()
        {
            Assert.IsNotNull(game, "Game should be initialized.");
            Assert.AreEqual(2, game.playerHand.Count, "Player should start with two cards.");
            Assert.AreEqual(2, game.dealerHand.Count, "Dealer should start with two cards.");
            Assert.AreEqual(2, game.conservativeHand.Count, "Conservative bot should start with two cards.");
        }

        [TestMethod]
        public void StartGameSetsGameStatusToTrue()
        {
            game.InitializeGame();
            Assert.IsTrue(game.gamestatus, "Game status should be set to true after starting the game.");
        }

        [TestMethod]
        public void PlayerBlackJackIdentifiedCorrectly()
        {
            game.playerHand = new List<Card> { new Card("Ace", "Spades", 11), new Card("King", "Hearts", 10) };
            bool result = game.IsBlackJack(game.playerHand);
            Assert.IsTrue(result, "Should recognize player has a blackjack.");
        }

        [TestMethod]
        public void PlayerBustIdentifiedCorrectly()
        {
            game.playerHand = new List<Card> { new Card("King", "Spades", 10), new Card("Queen", "Hearts", 10), new Card("2", "Clubs", 2) };
            bool result = game.IsBust(game.playerHand);
            Assert.IsTrue(result, "Should recognize player has busted.");
        }

        [TestMethod]
        public void HitAddsCardToHand()
        {
            var initialCount = game.playerHand.Count;
            game.Hit(game.playerHand);
            Assert.AreEqual(initialCount + 1, game.playerHand.Count, "Hit should add one card to the player's hand.");
        }

        [TestMethod]
        public void DealerStandsAtSeventeenOrHigher()
        {
            // Arrange
            game.dealerHand = new List<Card> {
                new Card("King", "Diamonds", 10),
                new Card("6", "Hearts", 6)
            };

            // Act
            game.Stand();

            // Assert
            var dealerHandValue = game.CalculateHandValue(game.dealerHand);
            Assert.IsTrue(dealerHandValue >= 17, "Dealer should stand at 17 or higher.");
        }

        [TestMethod]
        public void DealerHitsUntilSeventeenOrHigher()
        {
            // Arrange
            game.dealerHand = new List<Card> {
                new Card("King", "Diamonds", 10),
                new Card("5", "Hearts", 5)
            };

            game.deck.OverrideNextCard(new Card("Ace", "Diamonds", 1));

            // Act
            game.Stand();

            // Assert
            var dealerHandValue = game.CalculateHandValue(game.dealerHand);
            Assert.IsTrue(dealerHandValue >= 17, "Dealer should hit until reaching 17 or higher.");
        }

        [TestMethod]
        public void GameOutcome_PlayerWin()
        {
            // Arrange
            game.playerHand = new List<Card> {
                new Card("Ace", "Spades", 11),
                new Card("King", "Hearts", 10)
            };
            game.dealerHand = new List<Card> {
                new Card("10", "Diamonds", 10),
                new Card("9", "Hearts", 9)
            };

            // Act
            int result = game.gameOutcome(game.playerHand);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GameOutcome_DealerWin()
        {
            // Arrange
            game.playerHand = new List<Card> {
                new Card("10", "Spades", 10),
                new Card("9", "Hearts", 9)
            };
            game.dealerHand = new List<Card> {
                new Card("Ace", "Diamonds", 11),
                new Card("King", "Hearts", 10)
            };

            // Act
            int result = game.gameOutcome(game.playerHand);

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GameOutcome_Tie()
        {
            // Arrange
            game.playerHand = new List<Card> {
                new Card("9", "Spades", 9),
                new Card("King", "Hearts", 10)
            };
            game.dealerHand = new List<Card> {
                new Card("10", "Diamonds", 10),
                new Card("9", "Hearts", 9)
            };

            // Act
            int result = game.gameOutcome(game.playerHand);

            // Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GameOutcome_DealerBust()
        {
            // Arrange
            game.playerHand = new List<Card> {
                new Card("9", "Spades", 9),
                new Card("7", "Hearts", 7)
            };
            game.dealerHand = new List<Card> {
                new Card("King", "Diamonds", 10),
                new Card("Queen", "Hearts", 10),
                new Card("3", "Clubs", 3)
            };

            // Act
            int result = game.gameOutcome(game.playerHand);

            // Assert
            Assert.IsTrue(game.IsBust(game.dealerHand));
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void DealerBustsWhenHittingOverTwentyOne()
        {
            // Arrange
            game.dealerHand = new List<Card> {
                new Card("King", "Diamonds", 10),
                new Card("6", "Hearts", 6)
            };
            // Mock the deck to give a high-value card that will cause the dealer to bust
            game.deck.OverrideNextCard(new Card("6", "Spades", 6));

            // Act
            game.Stand();

            // Assert
            Assert.IsTrue(game.IsBust(game.dealerHand), "Dealer should bust when hitting over 21.");
        }

        [TestMethod]
        public void PlayerStandsAndGameContinuesToConservativeBot()
        {
            // Arrange
            game.playerHand = new List<Card> {
                new Card("10", "Spades", 10),
                new Card("7", "Hearts", 7)
            };
            // Stub player's choice to stand
            fakeServer.SetupReceive(2, "S");

            // Act
            game.playerDecision();

            // Assert
            Assert.AreEqual(1, game.turn, "Game should continue to conservative bot when player stands.");
        }

        [TestMethod]
        public void ConservativeBotHitsBasedOnStrategy()
        {
            // Arrange
            game.conservativeHand = new List<Card> {
                new Card("9", "Clubs", 9),
                new Card("2", "Diamonds", 2)
            };
            // Mock deck for controlled hit
            game.deck.OverrideNextCard(new Card("7", "Spades", 7));

            // Act
            game.ConservativeBot(game.conservativeHand);

            // Assert
            // The bot should hit based on the strategy, check if a card was added.
            Assert.AreEqual(3, game.conservativeHand.Count, "Conservative bot should hit based on strategy.");
        }

        [TestMethod]
        public void ConservativeBotStandsBasedOnStrategy()
        {
            // Arrange
            game.conservativeHand = new List<Card> {
                new Card("10", "Clubs", 10),
                new Card("7", "Diamonds", 7)
            };
            // Mock deck for controlled scenario
            game.deck.OverrideNextCard(new Card("7", "Spades", 7)); // This card shouldn't be drawn based on the bot's strategy

            // Act
            game.ConservativeBot(game.conservativeHand);

            // Assert
            // The bot should stand based on the strategy, check if a card was not added.
            Assert.AreEqual(2, game.conservativeHand.Count, "Conservative bot should stand based on strategy.");
        }

        [TestMethod]
        public void GameStopsWhenPlayerBusts()
        {
            // Arrange
            game.playerHand = new List<Card> {
                new Card("King", "Hearts", 10),
                new Card("Queen", "Spades", 10)
            };
            // Mock the deck to give a card that will cause the player to bust
            game.deck.OverrideNextCard(new Card("3", "Diamonds", 3));

            // Act
            game.Hit(game.playerHand); // This should cause the player to bust

            // Assert
            Assert.IsTrue(game.IsBust(game.playerHand), "Game should stop when player busts.");
        }

        [TestMethod]
        public void RunGame_ExecutesAllGamePhases()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Hearts", 10), new Card("7", "Clubs", 7) };
            game.conservativeHand = new List<Card> { new Card("9", "Spades", 9), new Card("7", "Diamonds", 7) };
            game.dealerHand = new List<Card> { new Card("8", "Hearts", 8), new Card("6", "Clubs", 6) };

            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(3, "H");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(3, "S");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");

            game.deck.OverrideNextCard(new Card("3", "Diamonds", 3));

            // Act
            game.StartGame();

            // Assert
            Assert.IsFalse(game.gamestatus, "Game status should be false after the game has completed.");
            Assert.AreEqual(2, game.turn, "After conservative bot and dealer, the turn should be reset or moved to the next player (if implemented).");
            var lastCard = game.playerHand.Last();
            Assert.AreEqual(3, lastCard.Value, "The last card in player's hand should be the card dealt ('3' of Diamonds).");
            Assert.IsFalse(game.IsBust(game.playerHand), "Player should not bust after hitting.");
            Assert.IsFalse(game.gamestatus, "Game should be completed and gamestatus set to false.");
            Assert.IsTrue(game.deck.RemainingCards() < initialDeckSize, "Cards should have been drawn from the deck.");
        }

        [TestMethod]
        public void RunGame_HandlesBlackJackCorrectly()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Hearts", 10), new Card("Ace", "Clubs", 11) };
            game.conservativeHand = new List<Card> { new Card("10", "Spades", 10), new Card("Ace", "Diamonds", 11) };
            game.dealerHand = new List<Card> { new Card("Ace", "Hearts", 11), new Card("10", "Clubs", 10) };

            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");

            // Act
            game.StartGame();

            // Assert
            Assert.IsFalse(game.gamestatus, "Game status should be false after the game has completed.");
            Assert.IsTrue(game.IsBlackJack(game.playerHand), "Player should be blackjack");
            Assert.IsTrue(game.IsBlackJack(game.conservativeHand), "Conservative should be blackjack");
            Assert.IsTrue(game.IsBlackJack(game.dealerHand), "Dealer should be blackjack");
            Assert.IsFalse(game.gamestatus, "Game should be completed and gamestatus set to false.");
        }

        [TestMethod]
        public void RunGame_HandlesCBotBustCorrectly()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Hearts", 10), new Card("Ace", "Clubs", 11) };
            game.conservativeHand = new List<Card> { new Card("6", "Spades", 6), new Card("Ace", "Diamonds", 11), new Card("5", "Spades", 5) };
            game.dealerHand = new List<Card> { new Card("Ace", "Hearts", 11), new Card("10", "Clubs", 10) };

            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");
            fakeServer.SetupReceive(2, "Continue");

            // Act
            game.StartGame();

            // Assert
            Assert.IsFalse(game.gamestatus, "Game status should be false after the game has completed.");
            Assert.IsTrue(game.IsBust(game.conservativeHand), "Conservative should should bust");
            Assert.IsFalse(game.gamestatus, "Game should be completed and gamestatus set to false.");
        }

        [TestMethod]
        public void CheckCBotBust_BotBustsCorrectly()
        {
            // Arrange
            // Set the conservative bot's hand to bust
            game.conservativeHand = new List<Card>
            {
                new Card("King", "Spades", 10),
                new Card("Queen", "Hearts", 10)
            };
            game.deck.OverrideNextCard(new Card("3", "Diamonds", 3));
            // Act
            game.Hit(game.conservativeHand); // Hit the bot once before checking for bust
            var result = game.CheckCBotBust();

            // Assert
            Assert.IsTrue(result, "Should return true if conservative bot busts.");

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                game.CheckCBotBust();

                var expectedOutput = "CBot Busted! Dealer's turn.";
                Assert.IsTrue(sw.ToString().Contains(expectedOutput), "Output should contain the expected bust message.");
            }
        }

        [TestMethod]
        public void checkCBotBJ_CBotGetsBlackjack()
        {
            // Arrange
            game.conservativeHand = new List<Card>
            {
                new Card("Ace", "Spades", 11),
                new Card("King", "Hearts", 10)
            };

            // Act
            var result = game.checkCBotBJ();

            // Assert
            Assert.IsTrue(result, "Should return true if conservative bot gets a blackjack.");
        }

        [TestMethod]
        public void playerDecision_PlayerHits()
        {
            // Arrange
            fakeServer.SetupReceive(2, "H");

            // Set up the player's hand and the next card to be dealt
            game.playerHand = new List<Card>
            {
                new Card("9", "Clubs", 9),
                new Card("2", "Hearts", 2) // A hand that would typically prompt a hit
            };
            game.deck.OverrideNextCard(new Card("5", "Diamonds", 5)); // The card that will be 'hit'

            var initialCount = game.playerHand.Count;

            // Act
            game.playerDecision();

            // Assert
            Assert.AreEqual(initialCount + 1, game.playerHand.Count, "Player's hand should have one more card after hitting.");
            //assert the last card in the hand is the one you set up to be dealt
            var lastCard = game.playerHand.Last();
            Assert.AreEqual(5, lastCard.Value, "The last card should be a 5 of Diamonds.");
        }

        [TestMethod]
        public void playerDecision_PlayerStands()
        {
            // Arrange
            // Set the initial game turn to the player's turn
            game.turn = 0;
            fakeServer.SetupReceive(2, "S"); // 'S' would be the signal for stand

            var initialHandCount = game.playerHand.Count;

            // Act
            game.playerDecision(); // This should process the stand decision and change the turn

            // Assert
            Assert.AreEqual(1, game.turn, "Turn should be 1 indicating it's now the conservative bot's turn.");

            // Assert that no cards were added to the player's hand
            Assert.AreEqual(initialHandCount, game.playerHand.Count, "Player's hand count should not change after standing.");
        }

        [TestMethod]
        public void checkPlayerBust_PlayerDoesNotBust()
        {
            // Arrange
            // Set the player's hand to a non-busting total
            game.playerHand = new List<Card>
            {
                new Card("9", "Clubs", 9),
                new Card("2", "Hearts", 2) // This should give a total of 11
            };

            // The turn should be set to the player's turn initially
            game.turn = 0;

            // Act
            game.Hit(game.playerHand);
            game.checkPlayerBust();

            // Assert
            // Assert that the game's turn is still 0, indicating the player can continue playing
            Assert.AreEqual(0, game.turn, "Turn should still be 0, indicating the player has not busted.");
        }
        
        [TestMethod]
        public void checkPlayerBust_PlayerDoesBust()
        {
            // Arrange
            // Set the player's hand to a non-busting total
            game.playerHand = new List<Card>
            {
                new Card("10", "Clubs", 10),
                new Card("10", "Hearts", 10)
            };

            game.deck.OverrideNextCard(new Card("2", "Hearts", 2));

            // The turn should be set to the player's turn initially
            game.turn = 0;

            // Act
            game.Hit(game.playerHand);
            game.checkPlayerBust();

            // Assert
            Assert.AreEqual(1, game.turn, "Turn should be 1, indicating the player has busted.");
        }

        [TestMethod]
        public void checkPlayerBJ_PlayerDoesNotGetBlackjack()
        {
            // Arrange
            game.playerHand = new List<Card>
            {
                new Card("10", "Hearts", 10),
                new Card("9", "Clubs", 9) // Total 19, not a blackjack
            };

            // Act
            var result = game.checkPlayerBJ();

            // Assert
            Assert.IsFalse(result, "Should return false if player does not get blackjack.");
        }

        [TestMethod]
        public void Listen_WaitsForSpecificPacket()
        {
            // Set up the fake server to simulate packet reception, and to eventually provide a packet with PacketType 2
            fakeServer.SetupReceive(1, "Player,1");
            fakeServer.SetupReceive(2, ""); // This packet should break the loop in Listen()

            // Act
            game.Listen(game.playerHand); // Should exit loop when PacketType 2 is received

            // Assert
            Assert.AreEqual(2, game.MMMServer.packet.PacketType, "Listen should exit when it receives a packet with PacketType 2.");
        }

        [TestMethod]
        public void runBJP_HandlesCardRequestForPlayer()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("Ace", "Hearts", 11), new Card("King", "Spades", 10) };
            fakeServer.SetupReceive(0, "Player,1"); // Assuming SetupReceive simulates receiving a packet
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.playerHand); // This would be triggered internally after a packet is received

            // Assert
            var expected = "Hearts,Ace,11"; // Assuming this is the format for your packet data
            Assert.AreEqual(expected, fakeServer.dataPayloadString, "The packet should contain the correct card information for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesCardRequestForDealer()
        {
            // Arrange
            game.dealerHand = new List<Card> { new Card("Ace", "Diamonds", 11), new Card("King", "Clubs", 10) };
            fakeServer.SetupReceive(0, "Dealer,1"); // Simulate the packet reception
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.playerHand); // Trigger the method processing the packet

            // Assert
            var expected = "Diamonds,Ace";
            Assert.AreEqual(expected, fakeServer.dataPayloadString, "The packet should contain the correct card information for the dealer.");
        }

        [TestMethod]
        public void runBJP_HandlesHandTotalRequestForPlayer()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("Ace", "Hearts", 11), new Card("King", "Spades", 10) };
            fakeServer.SetupReceive(1, "Player");
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.playerHand); // Process the hand total request

            // Assert
            var expectedHandValue = "21"; // Player has Blackjack
            Assert.AreEqual(expectedHandValue, fakeServer.dataPayloadString, "The packet should contain the correct hand value for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesHandTotalRequestForDealerAfterHit()
        {
            // Arrange
            game.dealerHand = new List<Card> { new Card("Hearts", "Ace", 11), new Card("Spades", "Ace", 11) };
            game.deck.OverrideNextCard(new Card("Clubs", "Ace", 11));
            fakeServer.SetupReceive(1, "Dealer,3");
            fakeServer.receivePacket();

            // Act
            game.Hit(game.dealerHand);
            game.runBJP(game.playerHand); // Process the hand total request

            // Assert
            var expectedHandValue = "13"; // Dealer has 13
            Assert.AreEqual(expectedHandValue, fakeServer.dataPayloadString, "The packet should contain the correct hand value for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesHandTotalRequestForDealerShowingSecondCardOnly()
        {
            // Arrange
            game.dealerHand = new List<Card> { new Card("Ace", "Diamonds", 11), new Card("King", "Clubs", 10) };
            fakeServer.SetupReceive(1, "Dealer,1");
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.playerHand); // Process the hand total request, second card only for dealer

            // Assert
            var expectedHandValue = "10"; // Only showing second card value
            Assert.AreEqual(expectedHandValue, fakeServer.dataPayloadString, "The packet should contain the correct value for the dealer's second card.");
        }
        
        [TestMethod]
        public void runBJP_HandlesHandTotalRequestForCBotShowingSecondCardOnly()
        {
            // Arrange
            game.conservativeHand = new List<Card> { new Card("Ace", "Diamonds", 11), new Card("King", "Clubs", 10) };
            fakeServer.SetupReceive(1, "Bot,1");
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.conservativeHand); // Process the hand total request, second card only for dealer

            // Assert
            var expectedHandValue = "10"; // Only showing second card value
            Assert.AreEqual(expectedHandValue, fakeServer.dataPayloadString, "The packet should contain the correct value for the dealer's second card.");
        }

        [TestMethod]
        public void runBJP_HandlesHandTotalRequestForCBotAfterHit()
        {
            // Arrange
            game.conservativeHand = new List<Card> { new Card("Hearts", "Ace", 11), new Card("Spades", "Ace", 11) };
            game.deck.OverrideNextCard(new Card("Clubs", "Ace", 11));
            fakeServer.SetupReceive(1, "Bot,3");
            fakeServer.receivePacket();

            // Act
            game.Hit(game.conservativeHand);
            game.runBJP(game.conservativeHand); // Process the hand total request

            // Assert
            var expectedHandValue = "13"; // bot has 13
            Assert.AreEqual(expectedHandValue, fakeServer.dataPayloadString, "The packet should contain the correct hand value for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesCardCountRequestForPlayer()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("Ace", "Hearts", 11), new Card("King", "Spades", 10) };
            fakeServer.SetupReceive(4, "Player");
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.playerHand); // Process the card count request

            // Assert
            var expectedCount = "2"; // Player has two cards
            Assert.AreEqual(expectedCount, fakeServer.dataPayloadString, "The packet should contain the correct card count for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesCardCountRequestForDealer()
        {
            // Arrange
            game.dealerHand = new List<Card> { new Card("Ace", "Diamonds", 11), new Card("King", "Clubs", 10) };
            fakeServer.SetupReceive(4, "Dealer");
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.playerHand); // Process the card count request

            // Assert
            var expectedCount = "2"; // Dealer also has two cards
            Assert.AreEqual(expectedCount, fakeServer.dataPayloadString, "The packet should contain the correct card count for the dealer.");
        }

        [TestMethod]
        public void runBJP_HandlesGameOutcomeRequestForPlayerBust()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Diamonds", 10), new Card("King", "Clubs", 10) };
            game.deck.OverrideNextCard(new Card("2", "Diamonds", 2));

            fakeServer.SetupReceive(5, "Player,Bust");
            fakeServer.receivePacket();

            // Act
            game.Hit(game.playerHand);
            game.checkPlayerBust();
            game.runBJP(game.playerHand); // Process the outcome request

            // Assert
            var expectedBool = "True";
            Assert.AreEqual(expectedBool, fakeServer.dataPayloadString, "The packet should contain the correct outcome for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesGameOutcomeRequestForPlayerBlackjack()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Diamonds", 10), new Card("King", "Clubs", 10) };
            game.deck.OverrideNextCard(new Card("Ace", "Diamonds", 1));

            fakeServer.SetupReceive(5, "Player,Blackjack");
            fakeServer.receivePacket();

            // Act
            game.Hit(game.playerHand);
            game.runBJP(game.playerHand); // Process the outcome request

            // Assert
            var expectedBool = "True";
            Assert.AreEqual(expectedBool, fakeServer.dataPayloadString, "The packet should contain the correct outcome for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesGameOutcomeRequestForPlayerWinnings()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Diamonds", 10), new Card("King", "Clubs", 10) };
            game.deck.OverrideNextCard(new Card("Ace", "Diamonds", 1));

            fakeServer.SetupReceive(5, "Dealer,0");
            fakeServer.receivePacket();

            player.bet = 50;

            // Act
            game.Hit(game.playerHand);
            game.runBJP(game.playerHand); // Process the outcome request

            // Assert
            var expectedWinnings = "100";
            Assert.AreEqual(expectedWinnings, fakeServer.dataPayloadString, "The packet should contain the correct winnings for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesGameOutcomeRequestForPlayerResult()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Diamonds", 10), new Card("King", "Clubs", 10) };
            game.dealerHand = new List<Card> { new Card("10", "Spades", 10), new Card("King", "Hearts", 10) };

            fakeServer.SetupReceive(5, "Dealer,1");
            fakeServer.receivePacket();

            // Act
            game.runBJP(game.playerHand); // Process the outcome request

            // Assert
            var expectedResult = "3";
            Assert.AreEqual(expectedResult, fakeServer.dataPayloadString, "The packet should contain the correct result for the player.");
        }

        [TestMethod]
        public void runBJP_HandlesGameOutcomeRequestForPlayerBalance()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Diamonds", 10), new Card("King", "Clubs", 10) };
            game.playerHand = new List<Card> { new Card("10", "Spades", 10), new Card("King", "Hearts", 10) };
            game.deck.OverrideNextCard(new Card("Ace", "Diamonds", 1));

            fakeServer.SetupReceive(5, "Dealer,0");

            player.bet = 50;

            // Act
            game.Hit(game.playerHand);
            fakeServer.packet = fakeServer.receivePacket();
            game.runBJP(game.playerHand); // Process the outcome request
            fakeServer.SetupReceive(5, "Dealer,2");
            fakeServer.packet = fakeServer.receivePacket();
            game.runBJP(game.playerHand);

            // Assert
            var expectedBalance = "1100";
            Assert.AreEqual(expectedBalance, fakeServer.dataPayloadString, "The packet should contain the correct balance for the player.");
        }

        [TestMethod]
        public void gameOutcome_PlayerWinAfterStand()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("10", "Diamonds", 10), new Card("King", "Clubs", 10) };
            game.dealerHand = new List<Card> { new Card("10", "Spades", 10), new Card("Nine", "Hearts", 9) };

            // Act
            int actual = game.gameOutcome(game.playerHand);

            // Assert
            int expectedResult = 1;
            Assert.AreEqual(expectedResult, actual, "The result should be 1 when the player wins with 2 cards.");
        }

        [TestMethod]
        public void gameOutcome_PlayerLoseAfterHitThenStand()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("5", "Diamonds", 5), new Card("King", "Clubs", 10) };
            game.dealerHand = new List<Card> { new Card("10", "Spades", 10), new Card("6", "Hearts", 6) };

            // Act
            int actual = game.gameOutcome(game.playerHand);

            // Assert
            int expectedResult = 2;
            Assert.AreEqual(expectedResult, actual, "The result should be 2 when the player loses with more than 2 cards without busting.");
        }

        [TestMethod]
        public void gameOutcome_PlayerWinAfterHitThenDealerBust()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("5", "Diamonds", 5), new Card("King", "Clubs", 10), new Card("5", "Clubs", 5) };
            game.dealerHand = new List<Card> { new Card("10", "Spades", 10), new Card("6", "Hearts", 6), new Card("King", "Spades", 10) };

            // Act
            int actual = game.gameOutcome(game.playerHand);

            // Assert
            int expectedResult = 1;
            Assert.AreEqual(expectedResult, actual, "The result should be 2 when the player loses with more than 2 cards without busting.");
        }

        [TestMethod]
        public void gameOutcome_PlayerLoseAfterHitThenDealerHit()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("5", "Diamonds", 5), new Card("King", "Clubs", 10), new Card("5", "Clubs", 5) };
            game.dealerHand = new List<Card> { new Card("10", "Spades", 10), new Card("6", "Hearts", 6), new Card("5", "Spades", 5) };

            // Act
            int actual = game.gameOutcome(game.playerHand);

            // Assert
            int expectedResult = 2;
            Assert.AreEqual(expectedResult, actual, "The result should be 2 when the player loses with more than 2 cards without busting.");
        }

        [TestMethod]
        public void gameOutcome_PushAfterHit()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("5", "Diamonds", 5), new Card("King", "Clubs", 10), new Card("5", "Clubs", 5) };
            game.dealerHand = new List<Card> { new Card("10", "Spades", 10), new Card("10", "Hearts", 10) };

            // Act
            int actual = game.gameOutcome(game.playerHand);

            // Assert
            int expectedResult = 3;
            Assert.AreEqual(expectedResult, actual, "The result should be 2 when the player loses with more than 2 cards without busting.");
        }

        [TestMethod]
        public void calculateHandValue_CorrectHandValueWithAces()
        {
            // Arrange
            game.playerHand = new List<Card> { new Card("Diamonds", "Ace", 11), new Card("Clubs", "Ace", 11), new Card("Spades", "Ace", 11) };

            // Act
            int actual = game.CalculateHandValue(game.playerHand);

            // Assert
            int expectedResult = 13;
            Assert.AreEqual(expectedResult, actual, "The result should be 13 when the player holds 3 aces.");
        }
    }
}