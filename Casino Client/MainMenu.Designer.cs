namespace Casino_Client
{
    partial class MainMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Showcard Gothic", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(180, 23);
            label1.Name = "label1";
            label1.Size = new Size(425, 33);
            label1.TabIndex = 0;
            label1.Text = "Welcome to MMM Casino 2.0";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Showcard Gothic", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(209, 67);
            label2.Name = "label2";
            label2.Size = new Size(369, 23);
            label2.TabIndex = 1;
            label2.Text = "Please choose an option below:";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.AutoSize = true;
            button1.BackColor = SystemColors.ActiveCaptionText;
            button1.Font = new Font("Showcard Gothic", 15.75F);
            button1.ForeColor = SystemColors.ButtonHighlight;
            button1.Location = new Point(160, 200);
            button1.Name = "button1";
            button1.Size = new Size(488, 80);
            button1.TabIndex = 2;
            button1.Text = "Blackjack";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button2.AutoSize = true;
            button2.BackColor = SystemColors.ActiveCaptionText;
            button2.Font = new Font("Showcard Gothic", 15.75F);
            button2.ForeColor = SystemColors.ButtonHighlight;
            button2.Location = new Point(160, 312);
            button2.Name = "button2";
            button2.Size = new Size(488, 80);
            button2.TabIndex = 3;
            button2.Text = "Roulette";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button3.AutoSize = true;
            button3.BackColor = SystemColors.ActiveCaptionText;
            button3.Font = new Font("Showcard Gothic", 15.75F);
            button3.ForeColor = SystemColors.ButtonHighlight;
            button3.Location = new Point(8, 8);
            button3.Name = "button3";
            button3.Size = new Size(72, 37);
            button3.TabIndex = 4;
            button3.Text = "QUIT";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button4.AutoSize = true;
            button4.BackColor = SystemColors.ActiveCaptionText;
            button4.Font = new Font("Showcard Gothic", 15.75F);
            button4.ForeColor = SystemColors.ButtonHighlight;
            button4.Location = new Point(680, 112);
            button4.Name = "button4";
            button4.Size = new Size(116, 37);
            button4.TabIndex = 5;
            button4.Text = "Settings";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(688, 16);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(100, 88);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Green;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "MainMenu";
            Text = "MainMenu";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private PictureBox pictureBox1;
    }
}