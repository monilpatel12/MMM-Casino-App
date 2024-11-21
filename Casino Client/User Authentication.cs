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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Casino_Client
{
    public partial class User_Authentication : Form
    {
        PlayerInfo player;
        TCPClient client;

        public User_Authentication(PlayerInfo p, TCPClient c)
        {
            InitializeComponent();

            player = p;
            client = c;

            hideFields();
            button4.Hide();
            button5.Hide();
            button6.Hide();
        }

        //Quit button
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Back button
        private void button5_Click(object sender, EventArgs e)
        {
            hideFields();
            button4.Hide();
            button5.Hide();
            button6.Hide();
            showOptions();
        }

        //Login button
        private void button1_Click(object sender, EventArgs e)
        {
            hideOptions();
            showFields();
            button4.Show();
            button5.Show();
        }

        //Login button 2 (Attempt to login)
        private void button4_Click(object sender, EventArgs e)
        {
            if (checkFields())
            {
                return;
            }

            else
            {
                client.packet = client.sendPacket(5, textBox1.Text + "," + textBox2.Text);
                client.packet = client.receivePacket();

                if (client.packet.PacketType == 0)
                {
                    loginSuccess();
                }
                else if (client.packet.PacketType == 1)
                {
                    textBox1.ForeColor = Color.Red;
                    textBox1.Text = client.dataPayloadString;
                }
                else
                {
                    textBox2.ForeColor = Color.Red;
                    textBox2.Text = client.dataPayloadString;
                }
            }
        }

        //Create Account button
        private void button2_Click(object sender, EventArgs e)
        {
            hideOptions();
            showFields();
            button5.Show();
            button6.Show();
        }

        //Create Account button 2 (Attempt to create account
        private void button6_Click(object sender, EventArgs e)
        {
            if (checkFields())
            {
                return;
            }

            else
            {
                client.packet = client.sendPacket(6, textBox1.Text + "," + textBox2.Text);
                client.packet = client.receivePacket();

                if (client.packet.PacketType == 0)
                {
                    loginSuccess();
                }
                else if (client.packet.PacketType == 1)
                {
                    textBox1.ForeColor = Color.Red;
                    textBox1.Text = client.dataPayloadString;
                }
            }
        }

        private bool checkFields()
        {
            if (textBox1.Text.Contains(",") || textBox1.Text.Contains(" ") || textBox2.Text.Contains(",") || textBox2.Text.Contains(" "))
            {
                label2.Text = "Invalid username or password format (no commas or spaces)";
                return true;
            }

            return false;
        }

        private void loginSuccess()
        {
            Image pfp;

            //Request server for pfp
            client.packet = client.sendPacket(4, "Requesting profile picture");
            client.packet = client.receiveImagePacket();

            using (var ms = new MemoryStream(client.packet.DataPayload))
            {
                pfp = Image.FromStream(ms);
            }

            var newform = new MainMenu(player, client, pfp);
            this.Hide();
            newform.Show();
        }

        private void showOptions()
        {
            button1.Show();
            button2.Show();
            label2.Text = "Please choose an option below:";
        }

        private void hideOptions()
        {
            button1.Hide();
            button2.Hide();
            label2.Text = "Enter Credentials";
        }

        private void showFields()
        {
            textBox1.Show();
            textBox2.Show();
            textBox1.Text = "Username";
            textBox2.Text = "Password";
            textBox1.ForeColor = Color.Gray;
            textBox2.ForeColor = Color.Gray;
            textBox2.PasswordChar = '\0';
        }

        private void hideFields()
        {
            textBox1.Hide();
            textBox2.Hide();
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox1.Text == "Username" || textBox1.Text == "Username is already taken" || textBox1.Text == "Invalid username")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox2.Text == "Password" || textBox2.Text == "Invalid password")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox2.PasswordChar = '*';
            textBox2.ForeColor = Color.Black;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "Password" || textBox2.Text == "Invalid password")
            {
                textBox2.PasswordChar = '\0';
                textBox2.ForeColor = Color.Gray;
            } 
        }
    }
}
