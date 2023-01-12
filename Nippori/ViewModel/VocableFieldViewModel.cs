using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Nippori.Bases;

namespace Nippori.ViewModel
{
    internal class VocableFieldViewModel : ModelBase
    {
        #region .: Private Variables :.

        private string text;
        private double fontSize = 40.0;
        private Brush background = Brushes.Transparent;
        private Brush foreground = SystemColors.WindowTextBrush;
        private bool kanjiFeaturesEnabled = true;

        #endregion

        #region .: Properties :.

        public string RtbDocumentXaml 
        { 
            get => ToFlowDocument(text, fontSize, SystemFonts.MessageFontFamily);
            set => NotifyPropertyChanged(nameof(RtbDocumentXaml));
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                RtbDocumentXaml = RtbDocumentXaml;
            }
        }

        public double FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                RtbDocumentXaml = RtbDocumentXaml;
            }
        }

        public Brush Background
        {
            get => background;
            set
            {
                background = value;
                NotifyPropertyChanged(nameof(Background));
            }
        }

        public Brush Foreground
        {
            get => foreground;
            set
            {
                foreground = value;
                NotifyPropertyChanged(nameof(Foreground));
            }
        }

        public bool KanjiFeaturesEnabled
        {
            get => kanjiFeaturesEnabled;
            set
            {
                kanjiFeaturesEnabled = value;
                NotifyPropertyChanged(nameof(KanjiFeaturesEnabled));
            }
        }

        #endregion

        #region .: Public Methods :.

        #endregion

        #region .: Private Methods :.

        private static string ToFlowDocument(string text, double fontSize, FontFamily fontFamily) =>
            "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TextAlignment=\"Center\" " + 
            $"FontFamily=\"{fontFamily}\" FontSize=\"{fontSize}\"><Paragraph>{text}</Paragraph></FlowDocument>";

        #endregion
    }
}
