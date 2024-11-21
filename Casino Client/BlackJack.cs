using CasinoLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Casino_Client
{
    public partial class BlackJack : Form
    {
        private PlayerInfo player;
        TCPClient client;
        Image pfp;

        private List<Image> p1Hand;
        private string p1HandTotal;
        private int numPlayer1Cards;
        List<PictureBox> playerCards;

        private List<Image> p2Hand;
        private string p2HandTotal;
        private string p2HandTotal2Cards;
        private int numPlayer2Cards;
        private int numPlayer2TotalCards = 2;

        private List<Image> p3Hand;
        private string p3HandTotal;
        private int numPlayer3Cards;
        private int numPlayer3TotalCards;

        private List<Image> p4Hand;
        private string p4HandTotal;
        private int numPlayer4Cards;
        private int numPlayer4TotalCards;

        List<PictureBox> botCards;

        List<PictureBox> dealerCards;
        private int numDealerCards;
        private int numDealerTotalCards = 2;

        public BlackJack(PlayerInfo p, TCPClient c, Image pfp)
        {
            player = p;
            client = c;
            this.pfp = pfp;

            p1Hand = new List<Image>();
            p2Hand = new List<Image>();
            p3Hand = new List<Image>();
            p4Hand = new List<Image>();

            p1HandTotal = "";
            p2HandTotal = "";
            p3HandTotal = "";
            p4HandTotal = "";

            numPlayer1Cards = 0;
            numDealerCards = 0;

            dealerCards = new List<PictureBox>();
            botCards = new List<PictureBox>();
            playerCards = new List<PictureBox>();

            InitializeComponent();
            InitializeStartupScreen();

            pictureBox12.Image = pfp;
            pictureBox13.Image = pfp;
            pictureBox12.BackColor = Color.Transparent;
        }

        private void InitializeStartupScreen()
        {
            // Create the panel
            startupPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Green // Use an appropriate color or background image
            };
            this.Controls.Add(startupPanel);

            // Create the Play button
            btnPlay = new Button()
            {
                Text = "Play",
                Size = new Size(100, 50),
                Location = new Point((this.ClientSize.Width - 100) / 2, (this.ClientSize.Height / 2) - 60)
            };
            btnPlay.Click += new EventHandler(btnPlay_Click);
            startupPanel.Controls.Add(btnPlay);

            // Create the Go Back button
            btnGoBack = new Button()
            {
                Text = "Go Back",
                Size = new Size(100, 50),
                Location = new Point((this.ClientSize.Width - 100) / 2, (this.ClientSize.Height / 2) + 10)
            };
            btnGoBack.Click += new EventHandler(btnGoBack_Click);
            startupPanel.Controls.Add(btnGoBack);

            // Hide game elements
            HideGameElements();
        }

        private void btnPlay_Click(object? sender, EventArgs e)
        {
            // Hide the startup panel
            startupPanel.Visible = false;

            UpdateBalance();

            // Show bet elements
            ShowBetElements();

            pictureBox12.Show();
            pictureBox12.Location = new Point(24, 216);
        }

        private void btnGoBack_Click(object? sender, EventArgs e)
        {
            player.bet = 0;

            this.Close();
            var newForm = new MainMenu(player, client, pictureBox12.Image);
            newForm.Show();
        }

        private void HideGameElements()
        {
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label7.Visible = false;
            button3.Visible = false;
            button6.Visible = false;
            button8.Visible = false;
            button10.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
            pictureBox10.Visible = false;

            foreach (Control c in this.Controls)
            {
                if (c is PictureBox)
                {
                    c.Visible = false; // This will hide the PictureBoxes that are dynamically added
                }
            }

            HideBetElements();
            hideGameEndElements();
        }

        private void ShowGameElements()
        {
            button3.Show();
            label2.Show();
            label3.Show();
        }

        private void ShowGameOptions()
        {
            button6.Show();
            button8.Show();
            label1.Show();
            label7.Show();
        }

        private void HideGameOptions()
        {
            button6.Hide();
            button8.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            player.bet = 0;

            this.Close();
            var newForm = new MainMenu(player, client, pictureBox12.Image);
            newForm.Show();
        }

        private void HideBetElements()
        {
            label4.Hide();
            label5.Hide();
            label6.Hide();
            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();
            pictureBox4.Hide();
            pictureBox5.Hide();
            pictureBox6.Hide();
            button3.Hide();
            button10.Hide();
            button11.Hide();
        }

        private void ShowBetElements()
        {
            label4.Show();
            label5.Show();
            label6.Show();
            pictureBox1.Show();
            pictureBox2.Show();
            pictureBox3.Show();
            pictureBox4.Show();
            pictureBox5.Show();
            pictureBox6.Show();
            button3.Show();
            button10.Show();
            button11.Show();

            pictureBox12.Image = pfp;
            pictureBox12.Location = new Point(24, 216);
            pictureBox12.Show();
        }

        private void updateBet()
        {
            label5.Text = player.bet.ToString();

            if (player.bet > 0 && player.balance > 0)
            {
                button10.Enabled = true;
                button10.ForeColor = Color.White;
            }

            else
            {
                button10.Enabled = false;
                button10.ForeColor = Color.DarkRed;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            player.bet += 1;
            updateBet();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            player.bet += 10;
            updateBet();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            player.bet += 25;
            updateBet();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            player.bet += 50;
            updateBet();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            player.bet += 100;
            updateBet();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            player.bet += 500;
            updateBet();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            player.bet = 0;
            updateBet();
        }

        //Initialize communications between client and server for a blackjack game
        private async void button10_Click(object sender, EventArgs e)
        {
            if (player.bet > 0)
            {
                HideBetElements();
                ShowGameElements();
                UpdateBalance();

                pictureBox12.Location = new Point(16, 232);
                pictureBox12.Show();

                //Send a request to start a game of blackjack
                client.packet = client.sendPacket(1, player.bet.ToString() + "," + player.balance.ToString());

                //Get the info from each player
                p1HandTotal = getPlayerInfo(p1Hand);
                client.packet = client.sendPacket(2, "Continue");   //Send a continue to the server to move on to next hand
                p2HandTotal2Cards = getPlayerInfo(p2Hand);
                p2HandTotal = p2HandTotal2Cards;

                //Send a request for the dealer's second card
                client.packet = client.sendPacket(0, "Dealer,2");
                client.packet = client.receivePacket();
                pictureBox8.Image = determineCard(0);

                //Send a request for the dealer's hand total
                client.packet = client.sendPacket(1, "Dealer,1");
                client.packet = client.receivePacket();
                label1.Text = client.dataPayloadString;

                numPlayer1Cards += 2;
                numDealerCards += 2;

                await dealCards();

                if (p1HandTotal == "21")
                {
                    p1HandTotal = "BlackJack!";
                    label8.Text = "BlackJack!";
                    HideGameOptions();

                    p2Turn();
                    dealerTurn();
                    await revealAllCards();
                }

                else
                {
                    ShowGameOptions();
                }

                //Notify server that client is finished
                client.packet = client.sendPacket(2, "");
            }
        }

        private void p2Turn()
        {
            //Send a request for player 2's card count
            client.packet = client.sendPacket(4, "Player");
            client.packet = client.receivePacket();
            numPlayer2TotalCards = Int32.Parse(client.dataPayloadString);

            //Check to see if the player got blackjack
            if (p2HandTotal == "21")
            {
                p2HandTotal = "BlackJack!";
            }

            //P2 loop for how many cards are left
            else if (numPlayer2TotalCards > 2)
            {
                for (int i = 3; i <= numPlayer2TotalCards; i++)
                {
                    numPlayer2Cards++;
                    client.packet = client.sendPacket(0, "Player," + i);
                    client.packet = client.receivePacket();
                    addBotCard(numPlayer2Cards);
                }
            }
        }

        private string getPlayerInfo(List<Image> hand)
        {
            //Send a request for the player's first card
            client.packet = client.sendPacket(0, "Player,1");
            client.packet = client.receivePacket();
            hand.Add(determineCard(0));

            //Send a request for the player's second card
            client.packet = client.sendPacket(0, "Player,2");
            client.packet = client.receivePacket();
            hand.Add(determineCard(0));

            //Send a request for the player's hand total
            client.packet = client.sendPacket(1, "Player,2");
            client.packet = client.receivePacket();
            string handTotal = client.dataPayloadString;

            return handTotal;
        }

        private async Task revealAllCards()
        {
            await Task.Delay(2000);
            for (int i = 0; i < playerCards.Count; i++)
            {
                playerCards[i].Hide();
            }
            label2.Text = "P2 Cards";
            label7.Text = p2HandTotal2Cards;
            pictureBox12.Image = Properties.Resources.ConservativePFP;
            pictureBox9.Image = p2Hand[0];
            pictureBox10.Image = p2Hand[1];

            for (int i = 0; i < numPlayer2TotalCards - 2; i++)
            {
                await Task.Delay(1000);
                botCards[i].Show();

                //Send a request for the player's new hand total
                client.packet = client.sendPacket(1, "Bot," + (i + 3));
                client.packet = client.receivePacket();
                label7.Text = client.dataPayloadString;
            }

            if (p2HandTotal == "21")
            {
                label7.Text = "Blackjack!";
            }

            await Task.Delay(2000);
            pictureBox11.Hide();
            pictureBox7.Show();

            //Send a request for the dealer's hand total
            client.packet = client.sendPacket(1, "Dealer,2");
            client.packet = client.receivePacket();
            label1.Text = client.dataPayloadString;

            //Continue to next player
            client.packet = client.sendPacket(2, "Continue");

            for (int i = 0; i < numDealerCards - 2; i++)
            {
                await Task.Delay(1000);
                dealerCards[i].Show();

                //Send a request for the dealer's new hand total
                client.packet = client.sendPacket(1, "Dealer," + (i + 3));
                client.packet = client.receivePacket();
                label1.Text = client.dataPayloadString;
            }

            //Continue to next player
            client.packet = client.sendPacket(2, "Continue");

            if (label1.Text == "21")
            {
                label1.Text = "Blackjack!";
            }

            await endScreen();
        }

        private void UpdateBalance()
        {
            player.balance = player.balance - player.bet;
            label6.Text = "Balance: " + player.balance;
        }

        private async Task dealCards()
        {
            //P1 card 1
            await Task.Delay(1000);
            pictureBox9.Image = p1Hand[0];
            pictureBox9.Show();
            await Task.Delay(1000);
            pictureBox9.Hide();
            label2.Text = "P2 Cards";
            pictureBox12.Image = Properties.Resources.ConservativePFP;

            //P2 card 1
            await Task.Delay(1000);
            pictureBox9.Image = p2Hand[0];
            pictureBox9.Show();
            await Task.Delay(1000);
            pictureBox9.Hide();
            label2.Text = "Your Cards";

            //Dealer card 1
            pictureBox12.Image = pfp;
            pictureBox9.Image = p1Hand[0];
            pictureBox9.Show();
            await Task.Delay(1000);
            pictureBox11.Show();

            //P1 card 1 and 2            
            await Task.Delay(1000);
            pictureBox10.Image = p1Hand[1];
            pictureBox10.Show();
            await Task.Delay(1000);
            pictureBox9.Hide();
            pictureBox10.Hide();
            label2.Text = "P2 Cards";

            //P2 card 1 and 2
            pictureBox12.Image = Properties.Resources.ConservativePFP;
            pictureBox9.Image = p2Hand[0];
            pictureBox9.Show();
            await Task.Delay(1000);
            pictureBox10.Image = p2Hand[1];
            pictureBox10.Show();
            await Task.Delay(1000);
            pictureBox9.Hide();
            pictureBox10.Hide();
            label2.Text = "Your Cards";

            //Dealer card 2
            pictureBox12.Image = pfp;
            pictureBox9.Image = p1Hand[0];
            pictureBox9.Show();
            pictureBox10.Image = p1Hand[1];
            pictureBox10.Show();
            await Task.Delay(1000);
            pictureBox8.Show();

            //Show hand totals
            await Task.Delay(1000);
            label1.Show();
            label7.Text = p1HandTotal;
            label7.Show();
            await Task.Delay(1000);
        }

        private Image determineCard(int index)
        {
            string[] cardInfo = client.dataPayloadString.Split(',');

            string rank = cardInfo[index];
            string suit = cardInfo[index + 1];

            Image determinedImage = Properties.Resources.ReverseSide;

            if (suit == "Clubs")
            {
                if (rank == "2")
                {
                    determinedImage = Properties.Resources.TwoClubs;
                }
                else if (rank == "3")
                {
                    determinedImage = Properties.Resources.ThreeClubs;
                }
                else if (rank == "4")
                {
                    determinedImage = Properties.Resources.FourClubs;
                }
                else if (rank == "5")
                {
                    determinedImage = Properties.Resources.FiveClubs;
                }
                else if (rank == "6")
                {
                    determinedImage = Properties.Resources.SixClubs;
                }
                else if (rank == "7")
                {
                    determinedImage = Properties.Resources.SevenClubs;
                }
                else if (rank == "8")
                {
                    determinedImage = Properties.Resources.EightClubs;
                }
                else if (rank == "9")
                {
                    determinedImage = Properties.Resources.NineClubs;
                }
                else if (rank == "10")
                {
                    determinedImage = Properties.Resources.TenClubs;
                }
                else if (rank == "Jack")
                {
                    determinedImage = Properties.Resources.JackClubs;
                }
                else if (rank == "Queen")
                {
                    determinedImage = Properties.Resources.QueenClubs;
                }
                else if (rank == "King")
                {
                    determinedImage = Properties.Resources.KingClubs;
                }
                else if (rank == "Ace")
                {
                    determinedImage = Properties.Resources.AceClubs;
                }
            }
            else if (suit == "Diamonds")
            {
                if (rank == "2")
                {
                    determinedImage = Properties.Resources.TwoDiamonds;
                }
                else if (rank == "3")
                {
                    determinedImage = Properties.Resources.ThreeDiamonds;
                }
                else if (rank == "4")
                {
                    determinedImage = Properties.Resources.FourDiamonds;
                }
                else if (rank == "5")
                {
                    determinedImage = Properties.Resources.FiveDiamonds;
                }
                else if (rank == "6")
                {
                    determinedImage = Properties.Resources.SixDiamonds;
                }
                else if (rank == "7")
                {
                    determinedImage = Properties.Resources.SevenDiamonds;
                }
                else if (rank == "8")
                {
                    determinedImage = Properties.Resources.EightDiamonds;
                }
                else if (rank == "9")
                {
                    determinedImage = Properties.Resources.NineDiamonds;
                }
                else if (rank == "10")
                {
                    determinedImage = Properties.Resources.TenDiamonds;
                }
                else if (rank == "Jack")
                {
                    determinedImage = Properties.Resources.JackDiamonds;
                }
                else if (rank == "Queen")
                {
                    determinedImage = Properties.Resources.QueenDiamonds;
                }
                else if (rank == "King")
                {
                    determinedImage = Properties.Resources.KingDiamonds;
                }
                else if (rank == "Ace")
                {
                    determinedImage = Properties.Resources.AceDiamonds;
                }
            }
            else if (suit == "Hearts")
            {
                if (rank == "2")
                {
                    determinedImage = Properties.Resources.TwoHearts;
                }
                else if (rank == "3")
                {
                    determinedImage = Properties.Resources.ThreeHearts;
                }
                else if (rank == "4")
                {
                    determinedImage = Properties.Resources.FourHearts;
                }
                else if (rank == "5")
                {
                    determinedImage = Properties.Resources.FiveHearts;
                }
                else if (rank == "6")
                {
                    determinedImage = Properties.Resources.SixHearts;
                }
                else if (rank == "7")
                {
                    determinedImage = Properties.Resources.SevenHearts;
                }
                else if (rank == "8")
                {
                    determinedImage = Properties.Resources.EightHearts;
                }
                else if (rank == "9")
                {
                    determinedImage = Properties.Resources.NineHearts;
                }
                else if (rank == "10")
                {
                    determinedImage = Properties.Resources.TenHearts;
                }
                else if (rank == "Jack")
                {
                    determinedImage = Properties.Resources.JackHearts;
                }
                else if (rank == "Queen")
                {
                    determinedImage = Properties.Resources.QueenHearts;
                }
                else if (rank == "King")
                {
                    determinedImage = Properties.Resources.KingHearts;
                }
                else if (rank == "Ace")
                {
                    determinedImage = Properties.Resources.AceHearts;
                }
            }
            else if (suit == "Spades")
            {
                if (rank == "2")
                {
                    determinedImage = Properties.Resources.TwoSpades;
                }
                else if (rank == "3")
                {
                    determinedImage = Properties.Resources.ThreeSpades;
                }
                else if (rank == "4")
                {
                    determinedImage = Properties.Resources.FourSpades;
                }
                else if (rank == "5")
                {
                    determinedImage = Properties.Resources.FiveSpades;
                }
                else if (rank == "6")
                {
                    determinedImage = Properties.Resources.SixSpades;
                }
                else if (rank == "7")
                {
                    determinedImage = Properties.Resources.SevenSpades;
                }
                else if (rank == "8")
                {
                    determinedImage = Properties.Resources.EightSpades;
                }
                else if (rank == "9")
                {
                    determinedImage = Properties.Resources.NineSpades;
                }
                else if (rank == "10")
                {
                    determinedImage = Properties.Resources.TenSpades;
                }
                else if (rank == "Jack")
                {
                    determinedImage = Properties.Resources.JackSpades;
                }
                else if (rank == "Queen")
                {
                    determinedImage = Properties.Resources.QueenSpades;
                }
                else if (rank == "King")
                {
                    determinedImage = Properties.Resources.KingSpades;
                }
                else if (rank == "Ace")
                {
                    determinedImage = Properties.Resources.AceSpades;
                }
            }

            return determinedImage;
        }

        //Hit button
        private async void button6_Click(object sender, EventArgs e)
        {
            //Run initial hit communications
            client.packet = client.sendPacket(3, "H");                                      //Send decision
            numPlayer1Cards++;
            client.packet = client.sendPacket(0, "Player," + numPlayer1Cards.ToString());    //Request next card
            client.packet = client.receivePacket();                                         //Receive next card
            addPlayerCard(numPlayer1Cards);

            //Check to see if the player has bust
            client.packet = client.sendPacket(5, "Player,Bust");    //Request game outcome
            client.packet = client.receivePacket();                 //Receive game outcome
            string isBust = client.dataPayloadString;

            //Check to see if the player got blackjack
            client.packet = client.sendPacket(5, "Player,Blackjack");   //Request game outcome
            client.packet = client.receivePacket();                     //Receive game outcome
            string isBJ = client.dataPayloadString;

            if (isBust == "True")
            {
                client.packet = client.sendPacket(2, "Continue");

                label8.Text = "You Have Bust";
                HideGameOptions();

                p2Turn();
                dealerTurn();
                await revealAllCards();
            }


            else if (isBJ == "True")
            {
                client.packet = client.sendPacket(2, "Continue");

                label7.Text = "Blackjack!";
                label8.Text = "BlackJack!";
                HideGameOptions();

                p2Turn();
                dealerTurn();
                await revealAllCards();
            }

            else
            {
                client.packet = client.sendPacket(2, "Continue");
            }
        }

        private async Task endScreen()
        {
            await Task.Delay(1000);

            label10.Text = "Hand Total: " + p1HandTotal;
            label12.Text = "Hand Total: " + p2HandTotal;
            label13.Text = "Hand Total: " + label1.Text;
            label6.Text = "Balance: " + player.balance;
            player.bet = 0;

            //Display information
            HideGameElements();
            showGameEndElements();
        }

        private void showGameEndElements()
        {
            button1.Show();
            button2.Show();
            panel1.Show();
            label8.Show();
        }

        private void hideGameEndElements()
        {
            button1.Hide();
            button2.Hide();
            panel1.Hide();
            label8.Hide();
        }

        private void addPlayerCard(int numCards)
        {
            PictureBox pictureBox = new PictureBox();

            if (numCards == 3)
            {
                pictureBox.Location = new Point(375, 274);
            }
            else if (numCards == 4)
            {
                pictureBox.Location = new Point(499, 274);
            }
            else
            {
                pictureBox.Location = new Point(623, 274);
            }

            pictureBox.BackColor = Color.Transparent;
            pictureBox.Size = new Size(136, 168);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.Image = determineCard(0);
            this.Controls.Add(pictureBox);
            pictureBox.BringToFront();
            pictureBox.Show();
            playerCards.Add(pictureBox);

            client.packet = client.sendPacket(1, "Player"); //Request hand value

            client.packet = client.receivePacket(); //Receive the hand total for the player

            string[] data = client.dataPayloadString.Split(',');
            label7.Text = data[0];
            p1HandTotal = label7.Text;
        }

        private void addDealerCard()
        {
            PictureBox pictureBox = new PictureBox();

            if (numDealerCards == 3)
            {
                pictureBox.Location = new Point(375, 31);
            }
            else if (numDealerCards == 4)
            {
                pictureBox.Location = new Point(499, 31);
            }
            else
            {
                pictureBox.Location = new Point(623, 31);
            }
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Size = new Size(136, 168);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.Image = determineCard(0);
            this.Controls.Add(pictureBox);
            pictureBox.BringToFront();
            pictureBox.Hide();

            dealerCards.Add(pictureBox);
        }

        private void addBotCard(int numCards)
        {
            PictureBox pictureBox = new PictureBox();

            if (numCards == 1)
            {
                pictureBox.Location = new Point(375, 274);
            }
            else if (numCards == 2)
            {
                pictureBox.Location = new Point(499, 274);
            }
            else
            {
                pictureBox.Location = new Point(623, 274);
            }
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Size = new Size(136, 168);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.Image = determineCard(0);
            this.Controls.Add(pictureBox);
            pictureBox.BringToFront();
            pictureBox.Hide();

            botCards.Add(pictureBox);

            string[] cardInfo = client.dataPayloadString.Split(',');
            p2HandTotal = (Int32.Parse(p2HandTotal) + Int32.Parse(cardInfo[2])).ToString(); ;
        }

        //Stand button
        private async void button8_Click(object sender, EventArgs e)
        {
            client.packet = client.sendPacket(3, "S"); //Send decision

            HideGameOptions();

            p2Turn();
            dealerTurn();
            await revealAllCards();
        }

        private void dealerTurn()
        {
            //Send a request for the dealer's first card
            client.packet = client.sendPacket(0, "Dealer,1");
            client.packet = client.receivePacket();
            pictureBox7.Image = determineCard(0);

            //Send a request for the dealer's card count
            client.packet = client.sendPacket(4, "Dealer");
            client.packet = client.receivePacket();
            numDealerTotalCards = Int32.Parse(client.dataPayloadString);

            //Dealer loop for how many cards are left
            if (Int32.Parse(client.dataPayloadString) > 2)
            {
                for (int i = 3; i <= numDealerTotalCards; i++)
                {
                    numDealerCards++;
                    client.packet = client.sendPacket(0, "Dealer," + i);
                    client.packet = client.receivePacket();
                    addDealerCard();
                }
            }

            client.packet = client.sendPacket(5, "Dealer,0");       //Request for game end payout
            client.packet = client.receivePacket();                 //Receive player's payout
            label11.Text = "Payout: " + client.dataPayloadString;

            client.packet = client.sendPacket(5, "Dealer,1");       //Request for game end results
            client.packet = client.receivePacket();                 //Receive the end results
            int result = Int32.Parse(client.dataPayloadString);
            switch (result)
            {
                case 0:
                    label8.Text = "Blackjack!";
                    break;

                case 1:
                    label8.Text = "You Win!";
                    break;

                case 2:
                    label8.Text = "You Lose!";
                    break;

                case 3:
                    label8.Text = "Push!";
                    break;

                case 4:
                    label8.Text = "You Have Bust";
                    break;
            }

            client.packet = client.sendPacket(5, "Dealer,2");       //Request for game end balance
            client.packet = client.receivePacket();                 //Receive the new balance
            player.balance = Int32.Parse(client.dataPayloadString);
        }

        //Restarts the game
        private void button1_Click(object sender, EventArgs e)
        {
            numPlayer1Cards = 0;
            numPlayer2Cards = 0;
            numPlayer3Cards = 0;
            numPlayer4Cards = 0;
            numDealerCards = 0;

            dealerCards = new List<PictureBox>();
            botCards = new List<PictureBox>();
            playerCards = new List<PictureBox>();

            p1Hand = new List<Image>();
            p2Hand = new List<Image>();
            p3Hand = new List<Image>();
            p4Hand = new List<Image>();

            p1HandTotal = "";
            p2HandTotal = "";
            p3HandTotal = "";
            p4HandTotal = "";

            label2.Text = "Your Cards";

            updateBet();

            HideGameElements();
            ShowBetElements();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            player.bet = 0;

            this.Close();
            var newForm = new MainMenu(player, client, pictureBox12.Image);
            newForm.Show();
        }
    }
}
