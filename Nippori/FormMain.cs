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

        /// <summary>
        /// Font určený pro asijské texty.
        /// </summary>
        private Font FontAsian = new Font("Microsoft JhengHei", 40);
        /// <summary>
        /// Font určený pro neasijské texty (tj. latinku a azbuku).
        /// </summary>
        private Font FontDefault = new Font("Segoe UI Semibold", 30);

        #endregion

        #region .: Private Fields :.

        private FormWaitPlease formWaitPlease;
        private VocableField[] vocableFields;

        private string currentFileName;

        private bool evaluating = false;
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
            CreateVocableFields();

            SetAssignmentFont();

            Vocabulary.Init();
        }

        #endregion

        #region .: Private Methods :.

        #region ... Controls

        private void CreateVocableFields()
        {
            vocableFields = new VocableField[] { vocableField1, vocableField2, vocableField3, vocableField4, vocableField5 };
        }

        /// <summary>
        /// Nastaví font zadání slovíčka podle použitého jazyka.
        /// </summary>
        private void SetAssignmentFont()
        {
            Font newFont;

            if (labelCzech.Text.Any(c => IsAsianCharacter(c)))
                newFont = FontAsian;
            else
                newFont = FontDefault;

            if (!labelCzech.Font.Equals(newFont))
                labelCzech.Font = newFont;
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
                item.Click += new EventHandler(toolStripMenuItem_Click);
            }

            ((ToolStripMenuItem)buttonTypes.DropDownItems[0]).Checked = true;
        }

        #endregion

        #region ... Utils

        /// <summary>
        /// Zjistí, jestli zadaný znak je z oblasti asijských znaků.
        /// </summary>
        /// <param name="c">Znak k posouzení.</param>
        /// <returns>TRUE nebo FALSE.</returns>
        private bool IsAsianCharacter(char c)
        {
            return (c >= 0x4E00 && c <= 0x9FCC) ||
                   (c >= 0x3400 && c <= 0x4DB5);
        }

        #endregion

        /// <summary>
        /// Vyhodnotí odpověď, případně zařadí slovíčko do fronty a vybere další.
        /// </summary>
        private void EvaluateAnswer()
        {
            bool result = true;

            foreach (VocableField vocableField in vocableFields.Where(vf => vf.Visible))
                result = vocableField.Evaluate() && result;

            if (result)
            {
                if (Vocabulary.QueueCount > 0)
                    Vocabulary.DequeueVocable();
                else
                    Vocabulary.GetNextVocable();
            }
            else
            {
                Vocabulary.EnqueueCurrentVocable();
                if (Vocabulary.QueueCount > 1)
                    Vocabulary.DequeueVocable();
                else
                    Vocabulary.GetNextVocable();
            }
        }

        /// <summary>
        /// Vybere ze slovníku další slovíčko a zobrazí ho.
        /// </summary>
        private void ShowNextVocable()
        {            
            Vocable vocable;

            vocable = Vocabulary.CurrentVocable;

            labelAssignment.Text = vocable.InputLabel;
            labelCzech.Text = vocable.Input;
            SetAssignmentFont();

            foreach (VocableField vocableField in vocableFields)
                vocableField.AssignedVocable = vocable;

            buttonAnswer.Enabled = true;
            vocableFields[0].Focus();
        }

        /// <summary>
        /// Spustí zkoušení.
        /// </summary>
        private void Start()
        {
            buttonAnswer.Visible = true;
            buttonOpen.Enabled = false;
            buttonTypes.Enabled = false;
            buttonGroups.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            labelAssignment.Visible = true;
            labelCzech.Visible = true;

            Vocabulary.Start();
            Vocabulary.GetNextVocable();
            ShowNextVocable();
            evaluating = false;
        }

        /// <summary>
        /// Zastaví zkoušení.
        /// </summary>
        private void Stop()
        {
            buttonAnswer.Visible = false;
            buttonOpen.Enabled = true;
            buttonTypes.Enabled = true;
            buttonGroups.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            labelAssignment.Visible = false;
            labelCzech.Visible = false;

            foreach (VocableField vf in vocableFields)
                vf.Hide();
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

        private void buttonAnswer_Click(object sender, EventArgs e)
        {
            if (!evaluating)
            {
                EvaluateAnswer();
                buttonAnswer.Text = "Další";
                evaluating = true;
            }
            else
            {
                buttonAnswer.Text = "Odpovědět";
                evaluating = false;
                ShowNextVocable();
            }
        }

        #endregion

    }
}
