using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibrary
{
    public class PlayerInfo
    {
        public double bet { get; set; }
        public double balance { get; set; }
        public double winnings { get; set; }

        public PlayerInfo() 
        { 
            bet = 0;
            balance = 1000;
            winnings = 0;
        }

        public void calculatePayout(int game, int result)
        {
            switch (game) 
            { 
                //blackjack
                case 0:
                    switch (result) 
                    {
                        //Blackjack + push or Blackjack + win
                        case 0:
                            winnings = (bet * 1.5) + (bet * 2);
                            balance += winnings;
                            break;

                        //Win
                        case 1:
                            winnings = bet * 2;
                            balance += winnings;
                            break;

                        //Lose
                        case 2:
                            winnings = 0;
                            balance += winnings;
                            break;

                        //Push
                        case 3:
                            winnings = bet;
                            balance += winnings;
                            break;

                        //Bust
                        case 4:
                            winnings = 0;
                            balance += winnings;
                            break;
                    }
                    break;

                case 1:
                    switch(result) 
                    {
                        //1 to 1 payout
                        case 0:
                            balance -= bet;
                            winnings = bet * 2;
                            balance += winnings;
                            break;
                        //Number
                        case 1:
                            balance -= bet;
                            winnings = bet * 36;
                            balance += winnings;
                            break;
                        //Lose
                        default:
                            balance -= bet;
                            winnings = 0;
                            balance += winnings;
                            break;
                    }
                    break;
            }
        }
    }
}
