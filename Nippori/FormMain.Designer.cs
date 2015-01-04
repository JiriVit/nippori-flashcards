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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.labelFileName = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonTypes = new System.Windows.Forms.ToolStripDropDownButton();
            this.item1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonGroups = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonStart = new System.Windows.Forms.ToolStripButton();
            this.buttonStop = new System.Windows.Forms.ToolStripButton();
            this.buttonTest = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelAssignment = new System.Windows.Forms.Label();
            this.labelCzech = new System.Windows.Forms.Label();
            this.buttonAnswer = new System.Windows.Forms.Button();
            this.vocableField1 = new Nippori.VocableField();
            this.vocableField2 = new Nippori.VocableField();
            this.vocableField3 = new Nippori.VocableField();
            this.vocableField4 = new Nippori.VocableField();
            this.vocableField5 = new Nippori.VocableField();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Tabulky Excel|*.xls;*.xlsx";
            this.openFileDialog.InitialDirectory = "D:\\Dokumenty\\Office\\Excel\\Vocabulary";
            this.openFileDialog.Title = "Otevřít lekci";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOpen,
            this.toolStripSeparator1,
            this.labelFileName,
            this.toolStripSeparator2,
            this.buttonTypes,
            this.buttonGroups,
            this.toolStripSeparator3,
            this.buttonStart,
            this.buttonStop,
            this.buttonTest});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Size = new System.Drawing.Size(1060, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonOpen
            // 
            this.buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpen.Image = global::Nippori.Properties.Resources.icon_open;
            this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(23, 22);
            this.buttonOpen.Text = "Otevřít soubor...";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // labelFileName
            // 
            this.labelFileName.Enabled = false;
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(86, 22);
            this.labelFileName.Text = "(žádný soubor)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonTypes
            // 
            this.buttonTypes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonTypes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.item1ToolStripMenuItem});
            this.buttonTypes.Enabled = false;
            this.buttonTypes.Image = ((System.Drawing.Image)(resources.GetObject("buttonTypes.Image")));
            this.buttonTypes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTypes.Name = "buttonTypes";
            this.buttonTypes.Size = new System.Drawing.Size(46, 22);
            this.buttonTypes.Tag = "T";
            this.buttonTypes.Text = "Typy";
            // 
            // item1ToolStripMenuItem
            // 
            this.item1ToolStripMenuItem.Name = "item1ToolStripMenuItem";
            this.item1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.item1ToolStripMenuItem.Text = "Item1";
            this.item1ToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // buttonGroups
            // 
            this.buttonGroups.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonGroups.Enabled = false;
            this.buttonGroups.Image = ((System.Drawing.Image)(resources.GetObject("buttonGroups.Image")));
            this.buttonGroups.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonGroups.Name = "buttonGroups";
            this.buttonGroups.Size = new System.Drawing.Size(62, 22);
            this.buttonGroups.Tag = "G";
            this.buttonGroups.Text = "Skupiny";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonStart
            // 
            this.buttonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonStart.Enabled = false;
            this.buttonStart.Image = global::Nippori.Properties.Resources.icon_start;
            this.buttonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(23, 22);
            this.buttonStart.Text = "Start";
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonStop.Enabled = false;
            this.buttonStop.Image = global::Nippori.Properties.Resources.icon_stop;
            this.buttonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(23, 22);
            this.buttonStop.Text = "Stop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonTest.Image = ((System.Drawing.Image)(resources.GetObject("buttonTest.Image")));
            this.buttonTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(23, 22);
            this.buttonTest.Text = "toolStripButton1";
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelAssignment, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCzech, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonAnswer, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.vocableField1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.vocableField2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.vocableField3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.vocableField4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.vocableField5, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1060, 576);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // labelAssignment
            // 
            this.labelAssignment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelAssignment.AutoSize = true;
            this.labelAssignment.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelAssignment.Location = new System.Drawing.Point(3, 16);
            this.labelAssignment.Name = "labelAssignment";
            this.labelAssignment.Size = new System.Drawing.Size(47, 17);
            this.labelAssignment.TabIndex = 0;
            this.labelAssignment.Text = "Zadání";
            this.labelAssignment.Visible = false;
            // 
            // labelCzech
            // 
            this.labelCzech.AutoSize = true;
            this.labelCzech.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCzech.Location = new System.Drawing.Point(3, 33);
            this.labelCzech.Name = "labelCzech";
            this.labelCzech.Size = new System.Drawing.Size(425, 54);
            this.labelCzech.TabIndex = 1;
            this.labelCzech.Text = "Příšerně žluťoučký kůň";
            this.labelCzech.Visible = false;
            // 
            // buttonAnswer
            // 
            this.buttonAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAnswer.Enabled = false;
            this.buttonAnswer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAnswer.Location = new System.Drawing.Point(925, 533);
            this.buttonAnswer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAnswer.Name = "buttonAnswer";
            this.buttonAnswer.Size = new System.Drawing.Size(132, 38);
            this.buttonAnswer.TabIndex = 6;
            this.buttonAnswer.Text = "Odpovědět";
            this.buttonAnswer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAnswer.UseVisualStyleBackColor = true;
            this.buttonAnswer.Click += new System.EventHandler(this.buttonAnswer_Click);
            // 
            // vocableField1
            // 
            this.vocableField1.AssignedVocable = null;
            this.vocableField1.CorrectAnswer = "odpoved";
            this.vocableField1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vocableField1.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.vocableField1.GivenAnswer = "odpověď";
            this.vocableField1.Icon = Nippori.VocableFieldIcon.ICON_NONE;
            this.vocableField1.ItemIndex = 0;
            this.vocableField1.ItemName = "(název sloupce)";
            this.vocableField1.Location = new System.Drawing.Point(3, 120);
            this.vocableField1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.vocableField1.Name = "vocableField1";
            this.vocableField1.Size = new System.Drawing.Size(1054, 75);
            this.vocableField1.TabIndex = 7;
            this.vocableField1.Visible = false;
            // 
            // vocableField2
            // 
            this.vocableField2.AssignedVocable = null;
            this.vocableField2.CorrectAnswer = "odpověď";
            this.vocableField2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vocableField2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.vocableField2.GivenAnswer = "odpověď";
            this.vocableField2.Icon = Nippori.VocableFieldIcon.ICON_NONE;
            this.vocableField2.ItemIndex = 1;
            this.vocableField2.ItemName = "(název sloupce)";
            this.vocableField2.Location = new System.Drawing.Point(3, 202);
            this.vocableField2.Name = "vocableField2";
            this.vocableField2.Size = new System.Drawing.Size(1054, 77);
            this.vocableField2.TabIndex = 8;
            this.vocableField2.Visible = false;
            // 
            // vocableField3
            // 
            this.vocableField3.AssignedVocable = null;
            this.vocableField3.CorrectAnswer = "odpověď";
            this.vocableField3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vocableField3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.vocableField3.GivenAnswer = "odpověď";
            this.vocableField3.Icon = Nippori.VocableFieldIcon.ICON_NONE;
            this.vocableField3.ItemIndex = 2;
            this.vocableField3.ItemName = "(název sloupce)";
            this.vocableField3.Location = new System.Drawing.Point(3, 285);
            this.vocableField3.Name = "vocableField3";
            this.vocableField3.Size = new System.Drawing.Size(1054, 77);
            this.vocableField3.TabIndex = 9;
            this.vocableField3.Visible = false;
            // 
            // vocableField4
            // 
            this.vocableField4.AssignedVocable = null;
            this.vocableField4.CorrectAnswer = "odpověď";
            this.vocableField4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vocableField4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.vocableField4.GivenAnswer = "odpověď";
            this.vocableField4.Icon = Nippori.VocableFieldIcon.ICON_NONE;
            this.vocableField4.ItemIndex = 3;
            this.vocableField4.ItemName = "(název sloupce)";
            this.vocableField4.Location = new System.Drawing.Point(3, 368);
            this.vocableField4.Name = "vocableField4";
            this.vocableField4.Size = new System.Drawing.Size(1054, 77);
            this.vocableField4.TabIndex = 10;
            this.vocableField4.Visible = false;
            // 
            // vocableField5
            // 
            this.vocableField5.AssignedVocable = null;
            this.vocableField5.CorrectAnswer = "odpověď";
            this.vocableField5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vocableField5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.vocableField5.GivenAnswer = "odpověď";
            this.vocableField5.Icon = Nippori.VocableFieldIcon.ICON_NONE;
            this.vocableField5.ItemIndex = 4;
            this.vocableField5.ItemName = "(název sloupce)";
            this.vocableField5.Location = new System.Drawing.Point(3, 451);
            this.vocableField5.Name = "vocableField5";
            this.vocableField5.Size = new System.Drawing.Size(1054, 77);
            this.vocableField5.TabIndex = 11;
            this.vocableField5.Visible = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 601);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Slovíčka";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelAssignment;
        private System.Windows.Forms.Button buttonAnswer;
        private System.Windows.Forms.Label labelCzech;
        private VocableField vocableField1;
        private System.Windows.Forms.ToolStripButton buttonOpen;
        private System.Windows.Forms.ToolStripDropDownButton buttonTypes;
        private System.Windows.Forms.ToolStripDropDownButton buttonGroups;
        private System.Windows.Forms.ToolStripButton buttonStart;
        private System.Windows.Forms.ToolStripButton buttonStop;
        private System.Windows.Forms.ToolStripMenuItem item1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel labelFileName;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton buttonTest;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private VocableField vocableField2;
        private VocableField vocableField3;
        private VocableField vocableField4;
        private VocableField vocableField5;
    }
}

