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
        #region .: Constants :.

        private Font FontChinese = new Font("Microsoft JhengHei", 40);
        private Font FontDefault = new Font("Segoe UI Semibold", 30);

        #endregion

        #region .: Private Fields :.

        private FormWaitPlease formWaitPlease;

        private string currentFileName;

        private bool running = false;
        private bool Evaluate = false;
        private Vocable previousVocable;

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

            SetAssignmentFont();

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

        /// <summary>
        /// Přepíše dostupné skupiny ze slovníku do menu.
        /// </summary>
        private void FillGroups()
        {
            ToolStripItem addedItem;
            buttonGroups.DropDownItems.Clear();

            foreach (string key in Vocabulary.Groups.Keys)
            {
                addedItem = buttonGroups.DropDownItems.Add(Vocabulary.Groups[key]);
                addedItem.Tag = key;
            }

            foreach (ToolStripMenuItem item in buttonGroups.DropDownItems)
            {                
                item.CheckOnClick = false;
                item.MouseEnter += new EventHandler(toolStripMenuItem_MouseEnter);
                item.MouseLeave += new EventHandler(toolStripMenuItem_MouseLeave);
                item.Click += new EventHandler(toolStripMenuItem_Click);
            }

            ((ToolStripMenuItem)buttonGroups.DropDownItems[0]).Checked = true;
        }

        /// <summary>
        /// Přepíše dostupné typy ze slovníku do menu.
        /// </summary>
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

        /// <summary>
        /// Zjistí, jestli zadaný znak je z čínské abecedy.
        /// </summary>
        /// <param name="c">Znak k posouzení.</param>
        /// <returns>TRUE nebo FALSE.</returns>
        private bool IsChineseCharacter(char c)
        {
            return (c >= 0x4E00 && c <= 0x9FCC) ||
                   (c >= 0x3400 && c <= 0x4DB5);
        }

        /// <summary>
        /// Nastaví font zadání slovíčka podle použitého jazyka.
        /// </summary>
        private void SetAssignmentFont()
        {
            Font newFont;

            if (labelCzech.Text.Any(c => IsChineseCharacter(c)))
                newFont = FontChinese;
            else
                newFont = FontDefault;

            if (!labelCzech.Font.Equals(newFont))
                labelCzech.Font = newFont;
        }

        /// <summary>
        /// Spustí zkoušení.
        /// </summary>
        private void Start()
        {
            running = true;

            buttonOpen.Enabled = false;
            buttonTypes.Enabled = false;
            buttonGroups.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        /// <summary>
        /// Zastaví zkoušení.
        /// </summary>
        private void Stop()
        {
            running = false;

            buttonOpen.Enabled = true;
            buttonTypes.Enabled = true;
            buttonGroups.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        /// <summary>
        /// Aktualizuje ve slovníku povolené skupiny a tipy na základě volby v menu.
        /// </summary>
        private void UpdateEnabledTypesAndGroups()
        {
            var checkedGroups = from item in buttonGroups.DropDownItems.OfType<ToolStripMenuItem>()
                                where item.Checked
                                select item.Tag.ToString();

            var checkedTypes = from item in buttonTypes.DropDownItems.OfType<ToolStripMenuItem>()
                               where item.Checked
                               select buttonTypes.DropDownItems.IndexOf(item) + 1;

            Vocabulary.EnabledGroups = checkedGroups.ToArray();
            Vocabulary.EnabledType = checkedTypes.ToArray()[0];
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
            string defaultFile = @"D:\Dokumenty\Office\Excel\Vocabulary\NCPR-12 slovíčka - pinyin (nová verze).xlsx";

            currentFileName = defaultFile;

            this.UseWaitCursor = true;
            backgroundWorker.RunWorkerAsync(defaultFile);
            formWaitPlease.ShowDialog();

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    currentFileName = openFileDialog.FileName;
            //    this.UseWaitCursor = true;
            //    MyTrace.WriteLine("info", currentFileName);
            //    backgroundWorker.RunWorkerAsync(currentFileName);
            //    formWaitPlease.ShowDialog();
            //}
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            /* sem zapiš testovací kód */
            SetAssignmentFont();
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

            UpdateEnabledTypesAndGroups();
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

            labelFileName.Text = Path.GetFileNameWithoutExtension(currentFileName);
            labelFileName.Enabled = true;
            labelFileName.Font = new Font(labelFileName.Font, FontStyle.Bold);

            FillTypes();
            FillGroups();
            UpdateEnabledTypesAndGroups();

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
