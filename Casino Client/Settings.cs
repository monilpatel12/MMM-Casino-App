using CasinoLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Casino_Client
{
    public partial class Settings : Form
    {
        PlayerInfo player;
        TCPClient tcpClient;

        public Settings(PlayerInfo p, TCPClient c, Image pfp)
        {
            InitializeComponent();

            this.player = p;
            this.tcpClient = c;

            pictureBox1.Image = pfp;
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);

            if (data != null)
            {
                var fileNames = data as string[];

                if (fileNames.Length > 0)
                {
                    pictureBox1.Image = Image.FromFile(fileNames[0]);
                }
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            pictureBox1.AllowDrop = true;
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            var newForm = new MainMenu(player, tcpClient, pictureBox1.Image);
            newForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Assuming pictureBox1 contains the image to be compressed and sent
            Image img = pictureBox1.Image;

            // Compress the Image
            using (MemoryStream ms = new MemoryStream())
            {
                // Save the image to the stream in JPEG format
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Convert to Byte Array
                byte[] imageBytes = ms.ToArray();

                // Send over TCP using your TCPClient
                tcpClient.packet = tcpClient.sendPacket(3, "Prepare for image transfer");
                tcpClient.packet = tcpClient.sendImagePacket(3, imageBytes);
            }
        }
    }
}
