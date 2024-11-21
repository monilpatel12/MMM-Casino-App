using System;
using System.Collections.Generic;
using System.Collections.Specialized;

/* Packet type guide for the blackjack game
 * 
 * Receiving
 * 0 - Request for a card, "Whos card, which card"
 * 1 - Request for hand total, "Whos hand, which cards to include (second argument is only for dealer)"
 * 2 - Notifying server to continue, "No message"
 * 3 - Decision, "H for hit or S for stand"
 * 4 - Request for card count, "Whos card count"
 * 5 - Request for game outcome, "Who's outcome, Bust Blackjack or Winlose"
 * 
 * Sending
 * 0 - Send a card, "Rank, Suit"
 * 1 - Send hand total, "Hand value"
 * 2 - Send card count, "Card count"
 * 3 - Send game outcome, "true or false based on the request"
 * 4 - Send ready status, "No message"
 * 
 */

namespace CasinoLibrary
{


    public class BlackjackGame
    {
        public Deck deck;
        public List<Card> playerHand;
        public List<Card> dealerHand;
        public List<Card> conservativeHand;
        public bool gamestatus;

        public TCPServer MMMServer;

        public int turn;            //0 For player, 1 for bot
        private bool playerBust = false;    
        public bool playerBJ = false;

        private PlayerInfo player;

        public BlackjackGame(TCPServer s, PlayerInfo p)
        {
            MMMServer = s;

            deck = new Deck();
            playerHand = new List<Card> { deck.DealCard(), deck.DealCard() };
            //playerHand = new List<Card> { new Card("Spades", "10", 10), new Card("Spades", "10", 10) };
            conservativeHand = new List<Card> { deck.DealCard(), deck.DealCard() };
            //conservativeHand = new List<Card> { new Card("Spades", "2", 2), new Card("Spades", "2", 2) };
            dealerHand = new List<Card> { deck.DealCard(), deck.DealCard() };

            //deck.OverrideNextCard(new Card("Spades", "2", 2));

            gamestatus = false;
            turn = 0;

            player = p;
        }

        public void StartGame()
        {
            InitializeGame();
            RunGame();
        }

        private void RunGame()
        {

            // Display hands of player and conservative bot
            DisplayHands();
            
            Listen(playerHand);
            MMMServer.packet.setPacket(27000, 27000, 4, MMMServer.packet.DataPayload);
            Listen(conservativeHand);

            while (gamestatus)
            {
                while (turn == 0) //Player
                {
                    if (checkPlayerBJ())
                    {
                        break;
                    }
                    playerDecision();
                    Listen(playerHand);
                }

                while (turn == 1) //Conservative Bot
                {
                    if (checkCBotBJ())
                    {
                        break;
                    }
                    ConservativeBot(conservativeHand);
                    if (CheckCBotBust())
                    {
                        break;
                    }
                }

                gamestatus = false;
            }
            gameOutcome(playerHand);
            gameOutcome(conservativeHand);

            //Listen for client's game outcome requests
            MMMServer.packet.setPacket(27000, 27000, 4, MMMServer.packet.DataPayload);
            Listen(conservativeHand);
            MMMServer.packet.setPacket(27000, 27000, 4, MMMServer.packet.DataPayload);
            Listen(dealerHand);
        }

        public bool CheckCBotBust()
        {
            if (IsBust(conservativeHand))
            {

                Console.WriteLine("CBot Busted! Dealer's turn.");

                // Call dealer after conservative bot's turn (stand uses the same functionality)
                Stand();

                return true;
            }

            return false;
        }

        public bool checkCBotBJ()
        {
            if (IsBlackJack(conservativeHand))
            {
                Console.WriteLine("BlackJack!");

                //Call dealer after conservative bot's turn (stand uses the same functionality)
                Stand();

                return true;
            }
            return false;
        }

        public void playerDecision()
        {
            //Wait for players decision
            Console.WriteLine("Awaiting response...");
            MMMServer.packet = MMMServer.receivePacket();

            string choice = MMMServer.dataPayloadString;

            switch (choice)
            {
                case "H":
                    Hit(playerHand);
                    checkPlayerBust();

                    break;
                case "S":
                    Console.WriteLine("Player chose to stand");
                    MMMServer.packet.setPacket(27000, 27000, 2, MMMServer.packet.DataPayload);
                    turn++;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        public void checkPlayerBust()
        {
            //Check for bust
            if (IsBust(playerHand))
            {
                Console.WriteLine("You busted! Next player's turn.");
                playerBust = true;

                // Call conservative bot after player's turn
                turn++;
            }
        }

        public bool checkPlayerBJ()
        {
            if (IsBlackJack(playerHand))
            {
                Console.WriteLine("BlackJack!");

                playerBJ = true;

                //Call conservative bot after player's turn
                turn++;

                return true;
            }

            return false;
        }

        public void Listen(List<Card> h)
        {
            List<Card> hand = h;

            //Listen for client's initial requests
            while (MMMServer.packet.PacketType != 2)
            {
                MMMServer.packet = MMMServer.receivePacket();
                runBJP(hand); //Runs a prtocol pased on received packet type
            }
        }

        public void InitializeGame()
        {
            gamestatus = true;
        }

        public void runBJP(List<Card> hand)
        {
            string[] data = MMMServer.dataPayloadString.Split(','); //Break down the request
            

            switch (MMMServer.packet.PacketType)
            {
                // Request for a card
                case 0:
                    if (data[0] == "Player")
                    {
                        MMMServer.packet = MMMServer.sendPacket(0, hand[Int32.Parse(data[1]) - 1].Rank + "," + hand[Int32.Parse(data[1]) - 1].Suit + "," + hand[Int32.Parse(data[1]) - 1].Value);
                    }
                    else if (data[0] == "Dealer")
                    {
                        MMMServer.packet = MMMServer.sendPacket(0, dealerHand[Int32.Parse(data[1]) - 1].Rank + "," + dealerHand[Int32.Parse(data[1]) - 1].Suit);
                    }
                    break;

                // Request for hand total
                case 1:
                    if (data[0] == "Player")
                    {
                        MMMServer.packet = MMMServer.sendPacket(1, CalculateHandValue(hand).ToString());
                    }
                    else if (data[0] == "Dealer")
                    {
                        int handTotal = 0;

                        if (data[1] == "1")
                        {
                            handTotal += dealerHand[1].Value;
                        }

                        else
                        {
                            for (int i = 0; i < Int32.Parse(data[1]); i++)
                            {
                                int numAces = 0;

                                handTotal += dealerHand[i].Value;
                                
                                if (dealerHand[i].Rank == "Ace")
                                {
                                    numAces++;
                                }

                                while (numAces > 0 && handTotal > 21)
                                {
                                    handTotal -= 10;
                                    numAces--;
                                }
                            }
                        }
                        
                        MMMServer.packet = MMMServer.sendPacket(1, handTotal.ToString());
                    }
                    else if (data[0] == "Bot")
                    {
                        int handTotal = 0;

                        if (data[1] == "1")
                        {
                            handTotal += hand[1].Value;
                        }

                        else
                        {
                            for (int i = 0; i < Int32.Parse(data[1]); i++)
                            {
                                int numAces = 0;

                                handTotal += hand[i].Value;

                                if (hand[i].Rank == "Ace")
                                {
                                    numAces++;
                                }

                                while (numAces > 0 && handTotal > 21)
                                {
                                    handTotal -= 10;
                                    numAces--;
                                }
                            }
                        }

                        MMMServer.packet = MMMServer.sendPacket(1, handTotal.ToString());
                    }
                    break;

                //Request for card count
                case 4:
                    if (data[0] == "Player")
                    {
                        MMMServer.packet = MMMServer.sendPacket(2, hand.Count.ToString());
                        MMMServer.packet.setPacket(27000, 27000, 4, MMMServer.packet.DataPayload);
                    }
                    else if (data[0] == "Dealer")
                    {
                        MMMServer.packet = MMMServer.sendPacket(2, dealerHand.Count.ToString());
                        MMMServer.packet.setPacket(27000, 27000, 4, MMMServer.packet.DataPayload);
                    }
                    break;

                //Request for game outcome
                case 5:
                    if (data[0] == "Player")
                    {
                        if (data[1] == "Bust")
                        {
                            MMMServer.packet = MMMServer.sendPacket(3, playerBust.ToString());
                        }
                        else if (data[1] == "Blackjack")
                        {
                            MMMServer.packet = MMMServer.sendPacket(3, IsBlackJack(hand).ToString());
                        }
                    }
                    else if (data[0] == "Dealer")
                    {
                        int result = gameOutcome(playerHand);
                        if (data[1] == "0")
                        {
                            player.calculatePayout(0, result);
                            MMMServer.packet = MMMServer.sendPacket(3, player.winnings.ToString());
                        }
                        else if (data[1] == "1")
                        {
                            MMMServer.packet = MMMServer.sendPacket(3, result.ToString());
                        }
                        else
                        {
                            MMMServer.packet = MMMServer.sendPacket(3, player.balance.ToString());
                        }
                    }
                    break;
            }
        } 

        private void DisplayHands(bool revealDealerHand = false)
        {
            Console.WriteLine("Player hand: " + CalculateHandValue(playerHand));
            foreach (Card card in playerHand)
            {
                Console.WriteLine($"{card.Rank} of {card.Suit}");
            }

            Console.WriteLine("CBots hand: " + CalculateHandValue(conservativeHand));
            foreach (Card card in conservativeHand)
            {
                Console.WriteLine($"{card.Rank} of {card.Suit}");
            }

            Console.WriteLine("\nDealer's hand: " + CalculateHandValue(dealerHand));
            if (!revealDealerHand)
            {
                // Hide the second card if it hasn't been revealed yet
                Console.WriteLine($"{dealerHand[0].Rank} of {dealerHand[0].Suit}");
                Console.WriteLine("Hidden Card");
            }
            else
            {
                // Reveal both cards if needed
                foreach (Card card in dealerHand)
                {
                    Console.WriteLine($"{card.Rank} of {card.Suit}");
                }
            }
        }

        public void Hit(List<Card> hand)
        {
            hand.Add(deck.DealCard());
            //Console.WriteLine("Hand: " + CalculateHandValue(hand));
        }

        public void Stand()
        {
            // Dealer hits until their hand value is at least 17
            while (CalculateHandValue(dealerHand) < 17)
            {
                dealerHand.Add(deck.DealCard());
            }

            // Reveal the entire dealer's hand
            DisplayHands(true);
        }

        public int gameOutcome(List<Card> hand)
        {
            //If player stands or player gets blackjack
            if (hand.Count == 2)
            {
                //Blackjack + push and Blackjack + win yield the same results
                if (IsBlackJack(hand))
                {
                    return 0;
                }
                //Player Win
                else if (CalculateHandValue(hand) > CalculateHandValue(dealerHand))
                {
                    return 1;
                }
                //Dealer has higher hand value
                else if (CalculateHandValue(hand) < CalculateHandValue(dealerHand))
                {
                    //Win by dealer busting
                    if (IsBust(dealerHand))
                    {
                        return 1;
                    }
                    //Dealer Win
                    else
                    {
                        return 2;
                    }
                }
                //Push
                else
                {
                    return 3;
                }
            }
            //If player hits
            else
            {
                //Player busts
                if (IsBust(hand))
                {
                    return 4;
                }
                //Player Win
                else if (CalculateHandValue(hand) > CalculateHandValue(dealerHand))
                {
                    return 1;
                }
                //Dealer has higher hand value
                else if (CalculateHandValue(hand) < CalculateHandValue(dealerHand))
                {
                    //Win by dealer busting
                    if (IsBust(dealerHand))
                    {
                        return 1;
                    }
                    //Dealer Win
                    else
                    {
                        return 2;
                    }
                }
                //Push
                else
                {
                    return 3;
                }
            }
        }
        public int CalculateHandValue(List<Card> hand)
        {
            int value = 0;
            int numAces = 0;

            foreach (Card card in hand)
            {
                value += card.Value;
                if (card.Rank == "Ace")
                {
                    numAces++;
                }
            }

            while (numAces > 0 && value > 21)
            {
                value -= 10;
                numAces--;
            }

            return value;
        }

        public bool IsBust(List<Card> hand)
        {
            if (CalculateHandValue(hand) > 21)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsBlackJack(List<Card> hand) 
        {
            if (CalculateHandValue(hand) == 21)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ConservativeBot(List<Card> conservativeHand)
        {
            int bustVal = 21;

            int handValue = CalculateHandValue(conservativeHand);
            int maximumDesiredCard = bustVal - handValue;
            int count = 0;

            int remainingCards = deck.RemainingCards();

            foreach (Card card in deck.Cards) // iterate over cards in the deck
            {
                if (card.Value <= maximumDesiredCard)
                {
                    count++;
                }
            }

            double likelihoodOfHitting = ((double)count / remainingCards) * 100; // Calculate likelihood of hitting
            if (likelihoodOfHitting > 75)
            {
                Hit(conservativeHand);
            }
            else
            {
                Stand();

                turn++;
            }
        }
    }
}