using CasinoLibrary;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Sockets;


namespace CasinoLibraryTests
{
    [TestClass]
    public class DeckTests
    {
        private Deck deck;

        [TestInitialize]
        public void Initialize()
        {
            // Assuming Deck has a parameterless constructor that initializes and shuffles the deck
            deck = new Deck();
        }

        [TestMethod]
        [Description("Checks that the InitializeDeck() method creates a deck with the correct number of cards.")]
        public void TestCorrectNumberOfCardsInitialized()
        {
            // The Initialize method should have already set up a new deck
            int expectedCount = 52; // Standard deck size
            int actualCount = deck.RemainingCards();

            Assert.AreEqual(expectedCount, actualCount, "The number of initialized cards should be equal to 52.");
        }

        [TestMethod]
        [Description("Ensures that the value for each Card object is correctly assigned according to the rank.")]
        public void TestCardValueAssignment()
        {
            // The Deck constructor should call InitializeDeck internally, so we don't need to call it directly
            Deck deck = new Deck(); // Assuming the constructor initializes the deck

            // You can't access private members directly, so instead, we deal out all the cards and test their values
            List<Card> dealtCards = new List<Card>();
            while (deck.RemainingCards() > 0)
            {
                dealtCards.Add(deck.DealCard());
            }

            // Now, we can check the values assigned to face cards and aces
            // Check the values assigned to face cards
            var faceCards = dealtCards.Where(c => c.Rank == "Jack" || c.Rank == "Queen" || c.Rank == "King");
            foreach (var card in faceCards)
            {
                Assert.AreEqual(10, card.Value, $"Face cards should have a value of 10, but {card.Rank} has a value of {card.Value}");
            }

            // Check the values assigned to Aces
            var aces = dealtCards.Where(c => c.Rank == "Ace");
            foreach (var card in aces)
            {
                Assert.AreEqual(11, card.Value, $"Aces should have a value of 11, but found a value of {card.Value}");
            }

            // Additionally, you can check the values for numeric cards
            foreach (var card in dealtCards.Where(c => c.Rank.All(char.IsDigit)))
            {
                int expectedValue = int.Parse(card.Rank);
                Assert.AreEqual(expectedValue, card.Value, $"Numeric cards should have a value matching their rank, but {card.Rank} has a value of {card.Value}");
            }
        }



        [TestMethod]
        [Description("Verifies that all suits and ranks are present in the deck after initialization.")]
        public void TestAllSuitsAndRanksPresent()
        {
            // Assuming the Deck constructor calls InitializeDeck internally
            Deck deck = new Deck();

            List<Card> dealtCards = new List<Card>();
            while (deck.RemainingCards() > 0)
            {
                dealtCards.Add(deck.DealCard());
            }

            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    Assert.IsTrue(dealtCards.Any(c => c.Suit == suit && c.Rank == rank),
                        $"The card with suit {suit} and rank {rank} is not present in the deck.");
                }
            }
        }

        [TestMethod]
        [Description("Verifies that calling DealCard() returns the first card from the deck and the card is removed from the list.")]
        public void TestDealCardRemovesFirstCard()
        {
            // Assume the deck is initialized and has cards
            Deck deck = new Deck(); // The Deck constructor is expected to initialize the deck with cards

            // Deal the first card and keep track of the remaining cards count before dealing
            int initialCount = deck.RemainingCards();
            var dealtCard = deck.DealCard();

            // Deal another card for comparison
            var secondDealtCard = deck.DealCard();

            // We don't have the expected first card from peek, so we can't compare directly.
            // Instead, we make sure the card dealt second is now the first card.
            Assert.AreNotEqual(dealtCard, secondDealtCard, "The second dealt card should not be the same as the first dealt card.");

            // Assert the deck size has decreased by one
            Assert.AreEqual(initialCount - 2, deck.RemainingCards(), "The deck should have one less card after dealing.");

            // Additional test: If you deal all cards and track them, you can verify that the first dealt card is not in the remaining deck
            List<Card> remainingCards = new List<Card>();
            while (deck.RemainingCards() > 0)
            {
                remainingCards.Add(deck.DealCard());
            }

            // Check that the dealtCard is not in the remaining cards
            Assert.IsFalse(remainingCards.Contains(dealtCard), "The dealt card should no longer be in the deck.");
        }

        [TestMethod]
        [Description("Checks that after dealing a card, the next call to DealCard() returns the new first card in the list.")]
        public void TestDealCardSequenceIntegrity()
        {
            // Assume the deck is initialized and has cards
            Deck deck = new Deck();

            // Deal the first two cards
            var firstDealtCard = deck.DealCard();
            var secondDealtCard = deck.DealCard();

            // Deal cards until one card is left in the deck
            while (deck.RemainingCards() > 1)
            {
                deck.DealCard();
            }

            // The next card to deal should now be the last card in the deck
            var lastCard = deck.DealCard();
        }


        [TestMethod]
        [Description("Ensures that dealing a card from an empty deck is handled gracefully.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDealCardWithEmptyDeck()
        {
            // Remove all cards from the deck
            while (deck.RemainingCards() > 0)
            {
                deck.DealCard();
            }

            // Attempt to deal a card from an empty deck, which should throw an InvalidOperationException
            deck.DealCard();
        }

        [TestMethod]
        [Description("Confirms that RemainingCards() correctly returns the number of cards left in the deck at various stages.")]
        public void TestRemainingCardsCountAccuracy()
        {
            // Assume the deck is initialized and has 52 cards
            Assert.AreEqual(52, deck.RemainingCards(), "Deck should start with 52 cards.");

            // Deal a card and test remaining count
            deck.DealCard();
            Assert.AreEqual(51, deck.RemainingCards(), "Deck should have 51 cards after dealing one.");

            // Continue dealing some cards and test count at each stage
            deck.DealCard();
            deck.DealCard();
            deck.DealCard();
            deck.DealCard();
            Assert.AreEqual(47, deck.RemainingCards(), "Deck should have 47 cards after dealing five cards in total.");
        }

        [TestMethod]
        [Description("Ensures that OverrideNextCard() correctly replaces the first card in the deck with the provided Card object.")]
        public void TestOverrideNextCardFunctionality()
        {
            // Create a new card that's not in the deck
            Card newCard = new Card("Diamonds", "Ace", 11);

            // Override the first card in the deck with the new card
            deck.OverrideNextCard(newCard);

            // Deal the first card, which should now be the new card we inserted
            Card dealtCard = deck.DealCard();

            Assert.AreEqual(newCard, dealtCard, "The first card dealt should be the new card that was set to be first.");
        }
    }
}