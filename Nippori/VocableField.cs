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
        /// Čte a zapisuje obsah popisku se správnou odpovědí a nastavuje jeho viditelnost.
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

        #endregion

        #region .: Private Fields :.

        private VocableFieldIcon myIcon = VocableFieldIcon.ICON_NONE;

        #endregion

        #region .: Constructors :.

        public VocableField()
        {
            InitializeComponent();
        }

        #endregion

        #region .: Event Handlers :.

        private void textBoxAnswer_KeyDown(object sender, KeyEventArgs e)
        {
            /* zatim nic, sem je potreba dopsat spravnou reakci na stisknuti
             * enteru */
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
