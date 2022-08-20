using System;
using System.Collections.Generic;
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
