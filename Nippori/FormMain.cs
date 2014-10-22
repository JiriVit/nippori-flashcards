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
        #region Private Fields

        private Label[] labelTransl;
        private Label[] labelCorr;
        private Label[] labelCorrect;
        private PictureBox[] pictureBox;
        private TextBox[] textBoxTransl;

        private bool Evaluate = false;
        private Vocable previousVocable;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Konstruktor formuláře.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            FillControlArrays();

            Vocabulary.Init();
            Vocabulary.GetRandomItem();
            ShowCurrentItem();
        }

        #endregion

        #region Private Methods

        private void EvaluateAnswer(bool newVocabulary)
        {
            int i;
            bool result = true;

            /* přetisknu správné odpovědi do příslušných polí */
            for (i = 0; i < 5; i++)
            {
                if (i < Vocabulary.CurrentItem.Type.Name.Length)
                {
                    if (textBoxTransl[i].Text.Equals(Vocabulary.CurrentItem.Translation[i]))
                    {
                        SetItemState(i, ItemState.Correct);
                    }
                    else
                    {
                        SetCorrectAnswerVisible(i, true);
                        SetItemState(i, ItemState.Wrong);
                        result = false;
                    }
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
        /// Naplní pole ovládacích prvků.
        /// </summary>
        private void FillControlArrays()
        {
            labelTransl = new Label[] { labelTransl1, labelTransl2, labelTransl3, labelTransl4, labelTransl5 };
            labelCorr = new Label[] { labelCorr1, labelCorr2, labelCorr3, labelCorr4, labelCorr5 };
            labelCorrect = new Label[] { labelCorrect1, labelCorrect2, labelCorrect3, labelCorrect4, labelCorrect5 };
            pictureBox = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5 };
            textBoxTransl = new TextBox[] { textBoxTransl1, textBoxTransl2, textBoxTransl3, textBoxTransl4, textBoxTransl5 };
        }

        /// <summary>
        /// Nastaví viditelnost pole pro odpověď.
        /// </summary>
        /// <param name="index">Index odpovědi (0..4).</param>
        /// <param name="visible">Požadovaná viditelnost.</param>
        private void SetAnswerVisible(int index, bool visible)
        {
            labelTransl[index].Visible = visible;
            textBoxTransl[index].Visible = visible;
            pictureBox[index].Visible = visible;
        }

        /// <summary>
        /// Nastaví viditelnost správné odpovědi.
        /// </summary>
        /// <param name="index">Index odpovědi (0..4).</param>
        /// <param name="visible">Požadovaná viditelnost.</param>
        private void SetCorrectAnswerVisible(int index, bool visible)
        {
            labelCorr[index].Visible = visible;
            labelCorrect[index].Visible = visible;
        }

        private void SetItemState(int index, ItemState itemState)
        {
            switch (itemState)
            {
                case ItemState.Correct:
                    pictureBox[index].Image = Properties.Resources.correct;
                    break;
                case ItemState.Empty:
                    pictureBox[index].Image = Properties.Resources.waiting;
                    break;
                case ItemState.Check:
                    pictureBox[index].Image = Properties.Resources.check;
                    break;
                case ItemState.Typing:
                    pictureBox[index].Image = Properties.Resources.typing;
                    break;
                case ItemState.Wrong:
                    pictureBox[index].Image = Properties.Resources.wrong;
                    break;
                default:
                    break;
            }
        }

        private void ShowCurrentItem()
        {
            int i;

            label1.Text = Vocabulary.CurrentItem.Type.InputName;
            labelCzech.Text = Vocabulary.CurrentItem.Czech;

            for (i = 0; i < 5; i++)
            {
                SetCorrectAnswerVisible(i, false);

                if (i < Vocabulary.CurrentItem.Type.Name.Length)
                {
                    SetAnswerVisible(i, true);
                    SetItemState(i, ItemState.Empty);
                    labelTransl[i].Text = Vocabulary.CurrentItem.Type.Name[i];
                    textBoxTransl[i].Text = "";
                    labelCorrect[i].Text = Vocabulary.CurrentItem.Translation[i];
                }
                else
                {
                    SetAnswerVisible(i, false);
                }
            }

            textBoxTransl[0].Focus();
        }

        #endregion

        #region Event Handlers

        private void textBoxTransl_Enter(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            SetItemState(textBox.TabIndex - 1, ItemState.Typing);
        }

        private void textBoxTransl_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == "")
                SetItemState(textBox.TabIndex - 1, ItemState.Empty);
            else
                SetItemState(textBox.TabIndex - 1, ItemState.Check);
        }

        private void textBoxTransl_KeyPress(object sender, KeyPressEventArgs e)
        {
            Control p;

            p = ((TextBox)sender).Parent;

            if (e.KeyChar == (char)Keys.Enter)
                p.SelectNextControl(ActiveControl, true, true, true, true);
        }

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
                ShowCurrentItem();
                buttonAnswer.Text = "Odpovědět";
                Evaluate = false;
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.UseWaitCursor = true;
                Vocabulary.ReadFile(openFileDialog.FileName);
                this.UseWaitCursor = false;
                this.Text = String.Format("Slovíčka - [{0}]", Path.GetFileNameWithoutExtension(openFileDialog.FileName));
                buttonAnswer.Enabled = true;

                Vocabulary.ReloadList();
                Vocabulary.GetRandomItem();

                if (Vocabulary.Configuration.ContainsKey("type") &&
                    Vocabulary.Configuration["type"].Equals("chinese"))
                {
                    labelCzech.Font = new System.Drawing.Font("SimSun", 50, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                }
                else
                {
                    labelCzech.Font = new System.Drawing.Font("Trebuchet MS", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                }


                ShowCurrentItem();
            }
        }

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
