using CasinoLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Casino_Client
{
    public partial class MainMenu : Form
    {
        private PlayerInfo player;
        TCPClient client;

        public MainMenu(PlayerInfo p, TCPClient c, Image pfp)
        {
            InitializeComponent();
            player = p;
            client = c;

            pictureBox1.Image = pfp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var newform = new BlackJack(player, client, pictureBox1.Image);
            this.Hide();
            newform.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var newform = new Roulette(player, client, pictureBox1.Image);
            this.Hide();
            newform.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var newform = new Settings(player, client, pictureBox1.Image);
            this.Hide();
            newform.Show();
        }
    }
}
