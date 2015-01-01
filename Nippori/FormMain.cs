using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nippori
{
    public partial class FormMain : Form
    {

        #region .: Private Fields :.

        private FormWaitPlease formWaitPlease;
        
        private Label[] labelTransl;
        private Label[] labelCorr;
        private Label[] labelCorrect;
        private PictureBox[] pictureBox;
        private TextBox[] textBoxTransl;

        private bool Evaluate = false;
        private Vocable previousVocable;

        private bool insideCheckedChanged = false;

        #endregion

        #region .: Constructor :.
        
        /// <summary>
        /// Konstruktor formuláře.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            MyTrace.Init();
            formWaitPlease = new FormWaitPlease();

            Vocabulary.Init();
            //Vocabulary.GetRandomItem();
        }

        #endregion

        #region .: Private Methods :.

        private void EvaluateAnswer(bool newVocabulary)
        {
            int i;
            bool result = true;

            /* přetisknu správné odpovědi do příslušných polí */
            for (i = 0; i < 5; i++)
            {
                if (i < Vocabulary.CurrentItem.Type.Name.Length)
                {
                    //if (textBoxTransl[i].Text.Equals(Vocabulary.CurrentItem.Translation[i]))
                    //{
                    //    SetItemState(i, ItemState.Correct);
                    //}
                    //else
                    //{
                    //    SetCorrectAnswerVisible(i, true);
                    //    SetItemState(i, ItemState.Wrong);
                    //    result = false;
                    //}
                }
            }

            /* podle výsledků zvolím další slovíčko */
            if (result)
            {
                Vocabulary.RemoveItem();
                if (Vocabulary.IsEmpty())
                {
                    Vocabulary.ReloadList();
                    do { Vocabulary.GetRandomItem(); } 
                    while (Vocabulary.CurrentItem == previousVocable);
                    previousVocable = Vocabulary.CurrentItem;
                    return;
                }
                if (Vocabulary.GetQueueCount() > 0)
                {
                    Vocabulary.GetQueueItem();
                    previousVocable = Vocabulary.CurrentItem;
                    return;
                }
                do
                {
                    Vocabulary.GetRandomItem();

                    if (Vocabulary.GetListCount() == 1)
                        break;

                    if (!Vocabulary.IsItemInQueue())
                        break;

                    if (!Vocabulary.CurrentItem.Equals(previousVocable))
                        break;
                }
                while (Vocabulary.IsItemInQueue() && (Vocabulary.GetListCount() != 1));
                previousVocable = Vocabulary.CurrentItem;
            }
            else
            {
                Vocabulary.PutToQueue();
                if (Vocabulary.GetQueueCount() == 2)
                {
                    Vocabulary.GetQueueItem();
                    previousVocable = Vocabulary.CurrentItem;
                    return;
                }
                Vocabulary.GetRandomItem();
                previousVocable = Vocabulary.CurrentItem;
            }


        }

        private void FillGroups()
        {
            buttonGroups.DropDownItems.Clear();

            foreach (string key in Vocabulary.Groups.Keys)
                buttonGroups.DropDownItems.Add(Vocabulary.Groups[key]);

            foreach (ToolStripMenuItem item in buttonGroups.DropDownItems)
            {
                item.CheckOnClick = false;
                item.MouseEnter += new EventHandler(toolStripMenuItem_MouseEnter);
                item.MouseLeave += new EventHandler(toolStripMenuItem_MouseLeave);
                item.Click += new EventHandler(toolStripMenuItem_Click);
            }

            ((ToolStripMenuItem)buttonGroups.DropDownItems[0]).Checked = true;
        }

        private void FillTypes()
        {
            buttonTypes.DropDownItems.Clear();

            foreach (VocableType type in Vocabulary.Types)
                buttonTypes.DropDownItems.Add(type.Name);

            foreach (ToolStripMenuItem item in buttonTypes.DropDownItems)
            {
                item.CheckOnClick = false;
                item.MouseEnter += new EventHandler(toolStripMenuItem_MouseEnter);
                item.MouseLeave += new EventHandler(toolStripMenuItem_MouseLeave);
                item.Click += new EventHandler(toolStripMenuItem_Click);
            }

            ((ToolStripMenuItem)buttonTypes.DropDownItems[0]).Checked = true;
        }

        #endregion

        #region .: Event Handlers :.

        #region ... Toolbar Buttons

        private void buttonAnswer_Click(object sender, EventArgs e)
        {
            if (!Evaluate)
            {
                EvaluateAnswer(true);
                buttonAnswer.Text = "Další";
                Evaluate = true;
            }
            else
            {
                buttonAnswer.Text = "Odpovědět";
                Evaluate = false;
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.UseWaitCursor = true;
                backgroundWorker.RunWorkerAsync(openFileDialog.FileName);
                formWaitPlease.ShowDialog();
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            /* sem zapiš testovací kód */
        }

        #endregion

        #region ... Toolbar Menu

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownMenu menu = (ToolStripDropDownMenu)(((ToolStripItem)sender).Owner);
            
            foreach (ToolStripMenuItem item in menu.Items)
            {
                item.Checked = item.Equals(sender);
            }
        }

        private void toolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            ((ToolStripDropDown)(((ToolStripItem)sender).Owner)).AutoClose = false;
        }

        private void toolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ((ToolStripDropDown)(((ToolStripItem)sender).Owner)).AutoClose = true;
        }

        #endregion

        #region ... BackgroundWorker

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = e.Argument.ToString();

            Vocabulary.ReadFile(fileName);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.UseWaitCursor = false;

            labelFileName.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            labelFileName.Enabled = true;
            labelFileName.Font = new Font(labelFileName.Font, FontStyle.Bold);

            FillTypes();
            FillGroups();

            buttonTypes.Enabled = true;
            buttonGroups.Enabled = true;
            buttonStart.Enabled = true;

            formWaitPlease.Close();
        }

        #endregion

        #region ... Form

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyTrace.Close();
        }

        #endregion

        #endregion

    }

    public enum ItemState
    {
        Hidden,
        Empty,
        Typing,
        Check,
        Correct,
        Wrong
    }

}
