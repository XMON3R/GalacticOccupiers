namespace SpaceInvadersALPHA
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            hrac = new PictureBox();
            bullet = new PictureBox();
            bulletTimer = new System.Windows.Forms.Timer(components);
            enemyTimer = new System.Windows.Forms.Timer(components);
            CollisionTimer = new System.Windows.Forms.Timer(components);
            en = new Label();
            score = new Label();
            hs = new Label();
            Exploze = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            explo = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)hrac).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bullet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)explo).BeginInit();
            SuspendLayout();
            // 
            // hrac
            // 
            hrac.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            hrac.Image = GalacticOccupiers.Properties.Resources.hrac;
            hrac.Location = new Point(590, 550);
            hrac.Name = "hrac";
            hrac.Size = new Size(100, 100);
            hrac.SizeMode = PictureBoxSizeMode.StretchImage;
            hrac.TabIndex = 0;
            hrac.TabStop = false;
            hrac.Click += hrac_Click;
            // 
            // bullet
            // 
            bullet.Location = new Point(0, 0);
            bullet.Name = "bullet";
            bullet.Size = new Size(5, 5);
            bullet.TabIndex = 3;
            bullet.TabStop = false;
            // 
            // bulletTimer
            // 
            bulletTimer.Interval = 1000;
            bulletTimer.Tick += bulletTimer_Tick;
            // 
            // enemyTimer
            // 
            enemyTimer.Tick += enemyTimer_Tick;
            // 
            // CollisionTimer
            // 
            CollisionTimer.Tick += CollisionTimer_Tick;
            // 
            // en
            // 
            en.AutoSize = true;
            en.ForeColor = Color.CornflowerBlue;
            en.Location = new Point(128, 611);
            en.Name = "en";
            en.Size = new Size(59, 15);
            en.TabIndex = 6;
            en.Text = "extra lives";
            // 
            // score
            // 
            score.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            score.AutoSize = true;
            score.ForeColor = Color.CornflowerBlue;
            score.Location = new Point(1104, 9);
            score.Name = "score";
            score.Size = new Size(35, 15);
            score.TabIndex = 7;
            score.Text = "skore";
            // 
            // hs
            // 
            hs.AutoSize = true;
            hs.ForeColor = Color.CornflowerBlue;
            hs.Location = new Point(49, 9);
            hs.Name = "hs";
            hs.Size = new Size(73, 15);
            hs.TabIndex = 8;
            hs.Text = "Highscore: 0";
            // 
            // Exploze
            // 
            Exploze.Tick += Exploze_Tick;
            // 
            // explo
            // 
            explo.Image = GalacticOccupiers.Properties.Resources.explo;
            explo.Location = new Point(319, 369);
            explo.Name = "explo";
            explo.Size = new Size(100, 50);
            explo.TabIndex = 9;
            explo.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Black;
            label1.ForeColor = Color.CornflowerBlue;
            label1.Location = new Point(1031, 569);
            label1.Name = "label1";
            label1.Size = new Size(119, 90);
            label1.TabIndex = 10;
            label1.Text = "enemy points:\r\nBLUE -> 10 points\r\nWHITE -> 20 points\r\nYELLOW -> 30 points\r\nPINK -> 40 points\r\nGREEN -> 50 points\r\n";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1264, 681);
            Controls.Add(label1);
            Controls.Add(explo);
            Controls.Add(hs);
            Controls.Add(score);
            Controls.Add(en);
            Controls.Add(bullet);
            Controls.Add(hrac);
            Name = "Form1";
            Text = "Galactic Occupiers™ by Simon";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown_1;
            ((System.ComponentModel.ISupportInitialize)hrac).EndInit();
            ((System.ComponentModel.ISupportInitialize)bullet).EndInit();
            ((System.ComponentModel.ISupportInitialize)explo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox hrac;
        private PictureBox bullet;
        private System.Windows.Forms.Timer bulletTimer;
        private System.Windows.Forms.Timer enemyTimer;
        private System.Windows.Forms.Timer CollisionTimer;
        private Label en;
        private Label score;
        private Label hs;
        private System.Windows.Forms.Timer Exploze;
        private System.Windows.Forms.Timer timer2;
        private PictureBox explo;
        private Label label1;
    }
}