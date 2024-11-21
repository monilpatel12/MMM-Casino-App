using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibrary
{
    public class Card
    {
        public string Suit { get; private set; }
        public string Rank { get; private set; }
        public int Value { get; private set; }

        public Card(string suit, string rank, int value)
        {
            Suit = suit;
            Rank = rank;
            Value = value;
        }
    }
}
