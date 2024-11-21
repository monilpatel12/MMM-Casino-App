using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CasinoLibrary;

namespace Casino_Client
{
    public partial class Roulette : Form
    {
        private PlayerInfo player;
        TCPClient client;
        PictureBox pictureBox;

        bool wheelIsMoved;
        float wheelTimes;
        System.Windows.Forms.Timer wheelTimer;
        RouletteWheel wheel;
        private float anglePerSlot;

        private string playerDecision;
        private int playerNumber;
        private string wheelOutcome;
        private string gameOutcome;

        public Roulette(PlayerInfo p, TCPClient c, Image pfp)
        {
            player = p;
            client = c;

            InitializeComponent();

            pictureBox9.Image = pfp;
            pictureBox10.Image = pfp;
            pictureBox11.Image = pfp;

            label3.Text = player.balance.ToString();
            label4.Text = player.bet.ToString();

            playerDecision = "";
            playerNumber = -1;
            wheelOutcome = "26";

            panel1.Hide();

            pictureBox = new PictureBox();
            pictureBox.Image = Properties.Resources.ChipEmpty;
            pictureBox.BackColor = Color.Transparent;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            this.Controls.Add(pictureBox);
            pictureBox.BringToFront();
            pictureBox.Hide();

            wheelTimer = new System.Windows.Forms.Timer();
            wheelTimer.Interval = 30; // speed 
            wheelTimer.Tick += wheelTimer_Tick;
            wheel = new RouletteWheel();

            client.packet = client.sendPacket(2, "Requesting to join roulette session");
        }

        public class RouletteWheel
        {
            public Bitmap picture;
            public Bitmap tempPicture;
            public float corner;
            public int[] stateValues;
            public int state;

            public RouletteWheel()
            {
                tempPicture = new Bitmap(Properties.Resources.rouletteWheel);
                picture = new Bitmap(Properties.Resources.rouletteWheel);
                stateValues = new int[] { 26, 3, 35, 12, 28, 7, 29, 18, 22, 9, 31, 14, 20, 1, 33, 16, 24, 5, 10, 23, 8, 30, 11, 36, 13, 27, 6, 34, 17, 25, 2, 21, 4, 19, 15, 32, 0 };
                corner = 0.0f;
            }

        }

        public static Bitmap RotateImage(Image image, float angle)
        {
            return RotateImage(image, new PointF((float)image.Width / 2, (float)image.Height / 2), angle);
        }

        public static Bitmap RotateImage(Image image, PointF offset, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");


            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            Graphics g = Graphics.FromImage(rotatedBmp);
            g.TranslateTransform(offset.X, offset.Y);
            g.RotateTransform(angle);
            g.TranslateTransform(-offset.X, -offset.Y);
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

        private void RotateImage(PictureBox pb, Image img, float angle)
        {
            if (img == null || pb.Image == null)
                return;

            Image oldImage = pb.Image;
            pb.Image = RotateImage(img, angle);
            if (oldImage != null)
            {
                oldImage.Dispose();
            }
        }

        private void wheelTimer_Tick(object sender, EventArgs e)
        {

            if (wheelIsMoved && wheelTimes > 0)
            {
                anglePerSlot = 360f / 37;
                wheel.corner += wheelTimes / 10;
                wheel.corner = wheel.corner % 360;
                RotateImage(pictureBox2, wheel.picture, wheel.corner);
                wheelTimes--;
            }

            wheel.state = Convert.ToInt32(Math.Floor(wheel.corner / anglePerSlot));
            wheelOutcome = Convert.ToString(wheel.stateValues[wheel.state]);

            if (wheelTimes == 0)
            {
                wheelTimer.Stop();
                client.packet = client.sendPacket(0, player.bet + "," + player.balance + "," + playerDecision + "," + wheelOutcome + "," + playerNumber);
                client.packet = client.receivePacket();

                string[] data = client.dataPayloadString.Split(',');

                pictureBox.Hide();

                label13.Text = playerDecision;
                label5.Text = "";

                label10.Text = player.bet.ToString();
                player.bet = 0;
                label4.Text = "0";

                player.winnings = Int32.Parse(data[0]);
                label15.Text = player.winnings.ToString();

                player.balance = Int32.Parse(data[1]);
                label3.Text = data[1];

                gameOutcome = data[2];
                label7.Text = data[2];

                label11.Text = data[3];

                panel1.Show();
            }
        }

        //Reset bet click event
        private void button3_Click(object sender, EventArgs e)
        {
            player.bet = 0;
            label4.Text = player.bet.ToString();
        }

        //Chip 1 click event
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            player.bet++;
            label4.Text = player.bet.ToString();
        }

        //Chip 10 click event
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            player.bet += 10;
            label4.Text = player.bet.ToString();
        }

        //Chip 25 click event
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            player.bet += 25;
            label4.Text = player.bet.ToString();
        }

        //Chip 50 click event
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            player.bet += 50;
            label4.Text = player.bet.ToString();
        }

        //Chip 100 click event
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            player.bet += 100;
            label4.Text = player.bet.ToString();
        }

        //Chip 500 click event
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            player.bet += 500;
            label4.Text = player.bet.ToString();
        }

        //Back button click event
        private void button7_Click(object sender, EventArgs e)
        {
            client.packet = client.sendPacket(1, "Player left roulette table, notifying server");

            player.bet = 0;

            this.Close();
            var newForm = new MainMenu(player, client, pictureBox9.Image);
            newForm.Show();
        }

        //1-18 click event
        private void oneto_Click(object sender, EventArgs e)
        {
            pictureBox.Location = new Point(512, 256);
            pictureBox.Size = new Size(120, 80);
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Show();

            label5.Text = "1 to 18";
            playerDecision = "1-18";
        }

        //19-36 click event
        private void nineteento_Click(object sender, EventArgs e)
        {
            pictureBox.Location = new Point(512, 352);
            pictureBox.Size = new Size(120, 80);
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Show();

            label5.Text = "19 to 36";
            playerDecision = "19-36";
        }

        //Even click event
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox.Location = new Point(672, 256);
            pictureBox.Size = new Size(120, 80);
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Show();

            label5.Text = "Even";
            playerDecision = "Even";
        }

        //Odd click event
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox.Location = new Point(672, 352);
            pictureBox.Size = new Size(120, 80);
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Show();

            label5.Text = "Odd";
            playerDecision = "Odd";
        }

        //Red click event
        private void red_Click(object sender, EventArgs e)
        {
            pictureBox.Location = new Point(832, 256);
            pictureBox.Size = new Size(120, 80);
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Show();

            label5.Text = "Red";
            playerDecision = "Red";
        }

        //Black click event
        private void black_Click(object sender, EventArgs e)
        {
            pictureBox.Location = new Point(832, 352);
            pictureBox.Size = new Size(120, 80);
            pictureBox.Show();

            label5.Text = "Black";
            playerDecision = "Black";
        }

        //Roulette table click event
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //Determine where the user clicked
            var mouseEventArgs = (MouseEventArgs)e;
            Point pictureBoxCoordinates = mouseEventArgs.Location;

            Point clickCoordinatesOnForm = new Point(pictureBoxCoordinates.X + 376, pictureBoxCoordinates.Y + 0);

            //Map click coords to a number
            MapCoordinatesToNumber(clickCoordinatesOnForm);
        }

        private int MapCoordinatesToNumber(Point coordinates)
        {
            Dictionary<Rectangle, int> numberAreas = new Dictionary<Rectangle, int>
            {
            { new Rectangle(424, 104, 32, 32), 0 },
            { new Rectangle(464, 160, 40, 64), 1 },
            { new Rectangle(464, 88, 40, 64), 2 },
            { new Rectangle(464, 24, 40, 64), 3 },
            { new Rectangle(504, 160, 40, 64), 4 },
            { new Rectangle(504, 88, 40, 64), 5 },
            { new Rectangle(504, 24, 40, 64), 6 },
            { new Rectangle(552, 160, 40, 64), 7 },
            { new Rectangle(552, 88, 40, 64), 8 },
            { new Rectangle(552, 24, 40, 64), 9 },
            { new Rectangle(592, 160, 40, 64), 10 },
            { new Rectangle(592, 88, 40, 64), 11 },
            { new Rectangle(592, 24, 40, 64), 12 },
            { new Rectangle(640, 160, 40, 64), 13 },
            { new Rectangle(640, 88, 40, 64), 14 },
            { new Rectangle(640, 24, 40, 64), 15 },
            { new Rectangle(680, 160, 40, 64), 16 },
            { new Rectangle(680, 88, 40, 64), 17 },
            { new Rectangle(680, 24, 40, 64), 18 },
            { new Rectangle(728, 160, 40, 64), 19 },
            { new Rectangle(728, 88, 40, 64), 20 },
            { new Rectangle(728, 24, 40, 64), 21 },
            { new Rectangle(768, 160, 40, 64), 22 },
            { new Rectangle(768, 88, 40, 64), 23 },
            { new Rectangle(768, 24, 40, 64), 24 },
            { new Rectangle(808, 160, 40, 64), 25 },
            { new Rectangle(808, 88, 40, 64), 26 },
            { new Rectangle(808, 24, 40, 64), 27 },
            { new Rectangle(856, 160, 40, 64), 28 },
            { new Rectangle(856, 88, 40, 64), 29 },
            { new Rectangle(856, 24, 40, 64), 30 },
            { new Rectangle(896, 160, 40, 64), 31 },
            { new Rectangle(896, 88, 40, 64), 32 },
            { new Rectangle(896, 24, 40, 64), 33 },
            { new Rectangle(944, 160, 40, 64), 34 },
            { new Rectangle(944, 88, 40, 64), 35 },
            { new Rectangle(944, 24, 40, 64), 36 },
            };

            foreach (var area in numberAreas.Keys)
            {
                if (area.Contains(coordinates))
                {
                    pictureBox.Location = area.Location;
                    pictureBox.Size = area.Size;

                    if (numberAreas[area] == 0)
                    {
                        pictureBox.BackColor = Color.Transparent;
                    }
                    else if (numberAreas[area] > 0 && numberAreas[area] < 10)
                    {
                        if (numberAreas[area] % 2 != 0)
                        {
                            pictureBox.BackColor = Color.FromArgb(217, 37, 27);
                        }
                        else
                        {
                            pictureBox.BackColor = Color.FromArgb(31, 25, 23);
                        }
                    }
                    else if (numberAreas[area] < 12)
                    {
                        pictureBox.BackColor = Color.FromArgb(31, 25, 23);
                    }
                    else if (numberAreas[area] < 19)
                    {
                        if (numberAreas[area] % 2 != 0)
                        {
                            pictureBox.BackColor = Color.FromArgb(31, 25, 23);
                        }
                        else
                        {
                            pictureBox.BackColor = Color.FromArgb(217, 37, 27);
                        }
                    }
                    else if (numberAreas[area] < 28)
                    {
                        if (numberAreas[area] % 2 != 0)
                        {
                            pictureBox.BackColor = Color.FromArgb(217, 37, 27);
                        }
                        else
                        {
                            pictureBox.BackColor = Color.FromArgb(31, 25, 23);
                        }
                    }
                    else if (numberAreas[area] < 30)
                    {
                        pictureBox.BackColor = Color.FromArgb(31, 25, 23);
                    }
                    else
                    {
                        if (numberAreas[area] % 2 != 0)
                        {
                            pictureBox.BackColor = Color.FromArgb(31, 25, 23);
                        }
                        else
                        {
                            pictureBox.BackColor = Color.FromArgb(217, 37, 27);
                        }
                    }

                    pictureBox.Show();

                    label5.Text = numberAreas[area].ToString();
                    playerDecision = "Number";
                    playerNumber = numberAreas[area];

                    return numberAreas[area];
                }
            }

            return -1;
        }

        //Spin click event
        private async void spinbutton_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(label4.Text) > 0 && playerDecision != "")
            {
                wheelIsMoved = true;
                Random rand = new Random();
                wheelTimes = rand.Next(150, 200);

                wheelTimer.Start();
            }
        }

        //Continue click event 
        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Hide();
        }
    }
}
