using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibrary
{
    public class Deck
    {
        private List<Card> cards;

        public Deck()
        {
            cards = new List<Card>();
            InitializeDeck();
            ShuffleDeck();
        }

        public List<Card> Cards
        {
            get { return cards; }
        }

        private void InitializeDeck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

            foreach (string suit in suits)
            {
                foreach (String rank in ranks)
                {
                    int value = (rank == "Ace") ? 11 : (Int32.TryParse(rank, out int num) ? num : 10);
                    cards.Add(new Card(suit: suit, rank: rank, value: value));
                }
            }
        }

        private void ShuffleDeck()
        {
            Random random = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        public Card DealCard()
        {
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }

        public int RemainingCards()
        {
            return cards.Count;
        }

        public void OverrideNextCard(Card card)
        {
            cards[0] = card;
        }
    }
}
