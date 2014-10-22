namespace Nippori
{
    partial class FormMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelCzech = new System.Windows.Forms.Label();
            this.labelTransl1 = new System.Windows.Forms.Label();
            this.textBoxTransl1 = new System.Windows.Forms.TextBox();
            this.labelTransl2 = new System.Windows.Forms.Label();
            this.textBoxTransl2 = new System.Windows.Forms.TextBox();
            this.labelTransl3 = new System.Windows.Forms.Label();
            this.labelTransl4 = new System.Windows.Forms.Label();
            this.labelTransl5 = new System.Windows.Forms.Label();
            this.textBoxTransl3 = new System.Windows.Forms.TextBox();
            this.textBoxTransl4 = new System.Windows.Forms.TextBox();
            this.textBoxTransl5 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelCorr1 = new System.Windows.Forms.Label();
            this.labelCorrect1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.labelCorr2 = new System.Windows.Forms.Label();
            this.labelCorr3 = new System.Windows.Forms.Label();
            this.labelCorr4 = new System.Windows.Forms.Label();
            this.labelCorr5 = new System.Windows.Forms.Label();
            this.labelCorrect2 = new System.Windows.Forms.Label();
            this.labelCorrect3 = new System.Windows.Forms.Label();
            this.labelCorrect4 = new System.Windows.Forms.Label();
            this.labelCorrect5 = new System.Windows.Forms.Label();
            this.buttonAnswer = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Česky";
            // 
            // labelCzech
            // 
            this.labelCzech.AutoSize = true;
            this.labelCzech.Font = new System.Drawing.Font("Trebuchet MS", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCzech.Location = new System.Drawing.Point(3, 40);
            this.labelCzech.Name = "labelCzech";
            this.labelCzech.Size = new System.Drawing.Size(182, 35);
            this.labelCzech.TabIndex = 1;
            this.labelCzech.Text = "(zvolte lekci)";
            // 
            // labelTransl1
            // 
            this.labelTransl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTransl1.AutoSize = true;
            this.labelTransl1.Location = new System.Drawing.Point(3, 152);
            this.labelTransl1.Name = "labelTransl1";
            this.labelTransl1.Size = new System.Drawing.Size(85, 18);
            this.labelTransl1.TabIndex = 2;
            this.labelTransl1.Text = "První překlad";
            // 
            // textBoxTransl1
            // 
            this.textBoxTransl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTransl1.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxTransl1.Location = new System.Drawing.Point(3, 173);
            this.textBoxTransl1.Name = "textBoxTransl1";
            this.textBoxTransl1.Size = new System.Drawing.Size(503, 30);
            this.textBoxTransl1.TabIndex = 1;
            this.textBoxTransl1.Text = "první odpověď";
            this.textBoxTransl1.Enter += new System.EventHandler(this.textBoxTransl_Enter);
            this.textBoxTransl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTransl_KeyPress);
            this.textBoxTransl1.Leave += new System.EventHandler(this.textBoxTransl_Leave);
            // 
            // labelTransl2
            // 
            this.labelTransl2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTransl2.AutoSize = true;
            this.labelTransl2.Location = new System.Drawing.Point(3, 232);
            this.labelTransl2.Name = "labelTransl2";
            this.labelTransl2.Size = new System.Drawing.Size(88, 18);
            this.labelTransl2.TabIndex = 2;
            this.labelTransl2.Text = "Druhý překlad";
            // 
            // textBoxTransl2
            // 
            this.textBoxTransl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTransl2.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxTransl2.Location = new System.Drawing.Point(3, 253);
            this.textBoxTransl2.Name = "textBoxTransl2";
            this.textBoxTransl2.Size = new System.Drawing.Size(503, 30);
            this.textBoxTransl2.TabIndex = 2;
            this.textBoxTransl2.Text = "první odpověď";
            this.textBoxTransl2.Enter += new System.EventHandler(this.textBoxTransl_Enter);
            this.textBoxTransl2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTransl_KeyPress);
            this.textBoxTransl2.Leave += new System.EventHandler(this.textBoxTransl_Leave);
            // 
            // labelTransl3
            // 
            this.labelTransl3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTransl3.AutoSize = true;
            this.labelTransl3.Location = new System.Drawing.Point(3, 312);
            this.labelTransl3.Name = "labelTransl3";
            this.labelTransl3.Size = new System.Drawing.Size(84, 18);
            this.labelTransl3.TabIndex = 2;
            this.labelTransl3.Text = "Třetí překlad";
            // 
            // labelTransl4
            // 
            this.labelTransl4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTransl4.AutoSize = true;
            this.labelTransl4.Location = new System.Drawing.Point(3, 392);
            this.labelTransl4.Name = "labelTransl4";
            this.labelTransl4.Size = new System.Drawing.Size(91, 18);
            this.labelTransl4.TabIndex = 2;
            this.labelTransl4.Text = "Čtvrtý překlad";
            // 
            // labelTransl5
            // 
            this.labelTransl5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTransl5.AutoSize = true;
            this.labelTransl5.Location = new System.Drawing.Point(3, 472);
            this.labelTransl5.Name = "labelTransl5";
            this.labelTransl5.Size = new System.Drawing.Size(80, 18);
            this.labelTransl5.TabIndex = 2;
            this.labelTransl5.Text = "Pátý překlad";
            // 
            // textBoxTransl3
            // 
            this.textBoxTransl3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTransl3.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxTransl3.Location = new System.Drawing.Point(3, 333);
            this.textBoxTransl3.Name = "textBoxTransl3";
            this.textBoxTransl3.Size = new System.Drawing.Size(503, 30);
            this.textBoxTransl3.TabIndex = 3;
            this.textBoxTransl3.Text = "první odpověď";
            this.textBoxTransl3.Enter += new System.EventHandler(this.textBoxTransl_Enter);
            this.textBoxTransl3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTransl_KeyPress);
            this.textBoxTransl3.Leave += new System.EventHandler(this.textBoxTransl_Leave);
            // 
            // textBoxTransl4
            // 
            this.textBoxTransl4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTransl4.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxTransl4.Location = new System.Drawing.Point(3, 413);
            this.textBoxTransl4.Name = "textBoxTransl4";
            this.textBoxTransl4.Size = new System.Drawing.Size(503, 30);
            this.textBoxTransl4.TabIndex = 4;
            this.textBoxTransl4.Text = "první odpověď";
            this.textBoxTransl4.Enter += new System.EventHandler(this.textBoxTransl_Enter);
            this.textBoxTransl4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTransl_KeyPress);
            this.textBoxTransl4.Leave += new System.EventHandler(this.textBoxTransl_Leave);
            // 
            // textBoxTransl5
            // 
            this.textBoxTransl5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTransl5.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxTransl5.Location = new System.Drawing.Point(3, 493);
            this.textBoxTransl5.Name = "textBoxTransl5";
            this.textBoxTransl5.Size = new System.Drawing.Size(503, 30);
            this.textBoxTransl5.TabIndex = 5;
            this.textBoxTransl5.Text = "první odpověď";
            this.textBoxTransl5.Enter += new System.EventHandler(this.textBoxTransl_Enter);
            this.textBoxTransl5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTransl_KeyPress);
            this.textBoxTransl5.Leave += new System.EventHandler(this.textBoxTransl_Leave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Nippori.Properties.Resources.waiting;
            this.pictureBox1.Location = new System.Drawing.Point(512, 173);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // labelCorr1
            // 
            this.labelCorr1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCorr1.AutoSize = true;
            this.labelCorr1.Location = new System.Drawing.Point(554, 152);
            this.labelCorr1.Name = "labelCorr1";
            this.labelCorr1.Size = new System.Drawing.Size(110, 18);
            this.labelCorr1.TabIndex = 2;
            this.labelCorr1.Text = "Správná odpověď";
            // 
            // labelCorrect1
            // 
            this.labelCorrect1.AutoSize = true;
            this.labelCorrect1.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCorrect1.Location = new System.Drawing.Point(554, 170);
            this.labelCorrect1.Name = "labelCorrect1";
            this.labelCorrect1.Size = new System.Drawing.Size(130, 24);
            this.labelCorrect1.TabIndex = 1;
            this.labelCorrect1.Text = "české slovíčko";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Nippori.Properties.Resources.waiting;
            this.pictureBox2.Location = new System.Drawing.Point(512, 253);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Nippori.Properties.Resources.waiting;
            this.pictureBox3.Location = new System.Drawing.Point(512, 333);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 32);
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::Nippori.Properties.Resources.waiting;
            this.pictureBox4.Location = new System.Drawing.Point(512, 413);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.TabIndex = 4;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::Nippori.Properties.Resources.waiting;
            this.pictureBox5.Location = new System.Drawing.Point(512, 493);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(32, 32);
            this.pictureBox5.TabIndex = 4;
            this.pictureBox5.TabStop = false;
            // 
            // labelCorr2
            // 
            this.labelCorr2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCorr2.AutoSize = true;
            this.labelCorr2.Location = new System.Drawing.Point(554, 232);
            this.labelCorr2.Name = "labelCorr2";
            this.labelCorr2.Size = new System.Drawing.Size(110, 18);
            this.labelCorr2.TabIndex = 2;
            this.labelCorr2.Text = "Správná odpověď";
            // 
            // labelCorr3
            // 
            this.labelCorr3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCorr3.AutoSize = true;
            this.labelCorr3.Location = new System.Drawing.Point(554, 312);
            this.labelCorr3.Name = "labelCorr3";
            this.labelCorr3.Size = new System.Drawing.Size(110, 18);
            this.labelCorr3.TabIndex = 2;
            this.labelCorr3.Text = "Správná odpověď";
            // 
            // labelCorr4
            // 
            this.labelCorr4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCorr4.AutoSize = true;
            this.labelCorr4.Location = new System.Drawing.Point(554, 392);
            this.labelCorr4.Name = "labelCorr4";
            this.labelCorr4.Size = new System.Drawing.Size(110, 18);
            this.labelCorr4.TabIndex = 2;
            this.labelCorr4.Text = "Správná odpověď";
            // 
            // labelCorr5
            // 
            this.labelCorr5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCorr5.AutoSize = true;
            this.labelCorr5.Location = new System.Drawing.Point(554, 472);
            this.labelCorr5.Name = "labelCorr5";
            this.labelCorr5.Size = new System.Drawing.Size(110, 18);
            this.labelCorr5.TabIndex = 2;
            this.labelCorr5.Text = "Správná odpověď";
            // 
            // labelCorrect2
            // 
            this.labelCorrect2.AutoSize = true;
            this.labelCorrect2.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCorrect2.Location = new System.Drawing.Point(554, 250);
            this.labelCorrect2.Name = "labelCorrect2";
            this.labelCorrect2.Size = new System.Drawing.Size(130, 24);
            this.labelCorrect2.TabIndex = 1;
            this.labelCorrect2.Text = "české slovíčko";
            // 
            // labelCorrect3
            // 
            this.labelCorrect3.AutoSize = true;
            this.labelCorrect3.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCorrect3.Location = new System.Drawing.Point(554, 330);
            this.labelCorrect3.Name = "labelCorrect3";
            this.labelCorrect3.Size = new System.Drawing.Size(130, 24);
            this.labelCorrect3.TabIndex = 1;
            this.labelCorrect3.Text = "české slovíčko";
            // 
            // labelCorrect4
            // 
            this.labelCorrect4.AutoSize = true;
            this.labelCorrect4.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCorrect4.Location = new System.Drawing.Point(554, 410);
            this.labelCorrect4.Name = "labelCorrect4";
            this.labelCorrect4.Size = new System.Drawing.Size(130, 24);
            this.labelCorrect4.TabIndex = 1;
            this.labelCorrect4.Text = "české slovíčko";
            // 
            // labelCorrect5
            // 
            this.labelCorrect5.AutoSize = true;
            this.labelCorrect5.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCorrect5.Location = new System.Drawing.Point(554, 490);
            this.labelCorrect5.Name = "labelCorrect5";
            this.labelCorrect5.Size = new System.Drawing.Size(130, 24);
            this.labelCorrect5.TabIndex = 1;
            this.labelCorrect5.Text = "české slovíčko";
            // 
            // buttonAnswer
            // 
            this.buttonAnswer.Enabled = false;
            this.buttonAnswer.Image = global::Nippori.Properties.Resources.settings;
            this.buttonAnswer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAnswer.Location = new System.Drawing.Point(3, 543);
            this.buttonAnswer.Name = "buttonAnswer";
            this.buttonAnswer.Size = new System.Drawing.Size(132, 39);
            this.buttonAnswer.TabIndex = 6;
            this.buttonAnswer.Text = "Odpovědět";
            this.buttonAnswer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAnswer.UseVisualStyleBackColor = true;
            this.buttonAnswer.Click += new System.EventHandler(this.buttonAnswer_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonAnswer, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.labelCzech, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCorrect5, 2, 11);
            this.tableLayoutPanel1.Controls.Add(this.labelCorr5, 2, 10);
            this.tableLayoutPanel1.Controls.Add(this.labelTransl1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCorr4, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.labelCorrect4, 2, 9);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTransl1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCorr3, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelTransl2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCorrect3, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelCorr2, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTransl5, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.labelCorr1, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCorrect2, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTransl2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelTransl5, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTransl4, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.labelCorrect1, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelTransl3, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTransl3, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelTransl4, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox5, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox4, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonOpen, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1061, 585);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOpen.Location = new System.Drawing.Point(965, 43);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(93, 42);
            this.buttonOpen.TabIndex = 7;
            this.buttonOpen.Text = "Otevřít...";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Tabulky Excel|*.xls;*.xlsx";
            this.openFileDialog.InitialDirectory = "D:\\Dokumenty\\Office\\Excel\\Vocabulary";
            this.openFileDialog.Title = "Otevřít lekci";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 585);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Slovíčka";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelCzech;
        private System.Windows.Forms.Label labelTransl1;
        private System.Windows.Forms.TextBox textBoxTransl1;
        private System.Windows.Forms.Label labelTransl2;
        private System.Windows.Forms.TextBox textBoxTransl2;
        private System.Windows.Forms.Label labelTransl3;
        private System.Windows.Forms.Label labelTransl4;
        private System.Windows.Forms.Label labelTransl5;
        private System.Windows.Forms.TextBox textBoxTransl3;
        private System.Windows.Forms.TextBox textBoxTransl4;
        private System.Windows.Forms.TextBox textBoxTransl5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelCorr1;
        private System.Windows.Forms.Label labelCorrect1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label labelCorr2;
        private System.Windows.Forms.Label labelCorr3;
        private System.Windows.Forms.Label labelCorr4;
        private System.Windows.Forms.Label labelCorr5;
        private System.Windows.Forms.Label labelCorrect2;
        private System.Windows.Forms.Label labelCorrect3;
        private System.Windows.Forms.Label labelCorrect4;
        private System.Windows.Forms.Label labelCorrect5;
        private System.Windows.Forms.Button buttonAnswer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

