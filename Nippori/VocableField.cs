using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nippori
{
    /// <summary>
    /// Pole pro zadání odpovědi, indikace stavu, správná odpověď.
    /// </summary>
    public partial class VocableField : UserControl
    {
        #region .: Properties :.

        /// <summary>
        /// Čte a zapisuje obsah popisku se správnou odpovědí. Současně nastavuje jeho viditelnost
        /// podle toho, je-li vložen prázdný řetězec.
        /// </summary>
        public string CorrectAnswer
        {
            get
            {
                return labelCorrectAnswer.Text;
            }
            set
            {
                labelCorrectAnswer.Text = value;

                if (labelCorrectAnswer.Text.Equals(String.Empty))
                    labelCorrectAnswer.Visible = labelCaptionCorrectAnswer.Visible = false;
                else
                    labelCorrectAnswer.Visible = labelCaptionCorrectAnswer.Visible = true;
            }
        }

        /// <summary>
        /// Čte a zapisuje obsah textového pole pro zadání odpovědi.
        /// </summary>
        public string GivenAnswer
        {
            get { return textBoxAnswer.Text; }
            set { textBoxAnswer.Text = value; }
        }

        /// <summary>
        /// Zapisuje a čte ikonu zobrazenou vedle pole pro zadání odpovědi.
        /// </summary>
        public VocableFieldIcon Icon
        {
            get 
            { 
                return myIcon; 
            }
            set 
            {
                myIcon = value;
                switch (myIcon)
                {
                    case VocableFieldIcon.ICON_OK:
                        pictureBox.Image = Properties.Resources.correct;
                        break;
                    case VocableFieldIcon.ICON_FAIL:
                        pictureBox.Image = Properties.Resources.wrong;
                        break;
                    default:
                        pictureBox.Image = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Čte a zapisuje popisek nad polem pro zadání odpovědi.
        /// </summary>
        public string ItemName
        {
            get { return labelItemName.Text;}
            set { labelItemName.Text = value; }
        }

        /// <summary>
        /// Čte a zapisuje slovíčko přiřazené tomuto poli.
        /// </summary>
        public Vocable AssignedVocable {
            get { return assignedVocable; }
            set
            {
                assignedVocable = value;

                if (assignedVocable != null)
                {
                    ItemName = assignedVocable.GetOutputLabel(ItemIndex);
                    Icon = VocableFieldIcon.ICON_NONE;
                    GivenAnswer = String.Empty;
                    CorrectAnswer = String.Empty;
                    Visible = (ItemIndex < assignedVocable.GetOutputCount());
                }
                else
                {
                    Visible = false;
                }
            }
        }

        /// <summary>
        /// Čte a zapisuje index položky slovíčka (neboli sloupce v tabulce slovíček).
        /// </summary>
        public int ItemIndex { get; set; }

        #endregion

        #region .: Private Fields :.

        private VocableFieldIcon myIcon = VocableFieldIcon.ICON_NONE;
        private Vocable assignedVocable;

        #endregion

        #region .: Constructors :.

        /// <summary>
        /// Vytvoří novou instanci třídy.
        /// </summary>
        public VocableField()
        {
            InitializeComponent();
        }

        #endregion

        #region .: Public Methods :.

        /// <summary>
        /// Vyhodnotí správnost zadaného slovíčka, podle toho zobrazí ikonu a správnou odpověď.
        /// </summary>
        /// <returns>Správnost zadaného slovíčka.</returns>
        public bool Evaluate()
        {
            if (!Visible)
                return true;

            if (GivenAnswer.Equals(assignedVocable.GetOutput(ItemIndex)))
            {
                Icon = VocableFieldIcon.ICON_OK;
                return true;
            }
            else
            {
                Icon = VocableFieldIcon.ICON_FAIL;
                CorrectAnswer = assignedVocable.GetOutput(ItemIndex);
                return false;
            }
        }

        #endregion

        #region .: Event Handlers :.

        private void textBoxAnswer_KeyDown(object sender, KeyEventArgs e)
        {
            /* TODO Dopsat správnou reakci na stisknutí Enteru, aby to nevydalo
             * chybový zvuk a aby to poskočilo na další pole. */
        }

        #endregion
    }

    /// <summary>
    /// Výčet ikon zobrazených vedle políčka pro odpověď.
    /// </summary>
    public enum VocableFieldIcon
    {
        /// <summary>
        /// Žádná ikona.
        /// </summary>
        ICON_NONE,
        /// <summary>
        /// Zelená ikona, správná odpověď.
        /// </summary>
        ICON_OK,
        /// <summary>
        /// Červená ikona, špatná odpověď.
        /// </summary>
        ICON_FAIL
    }
}
