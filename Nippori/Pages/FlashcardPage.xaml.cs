using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Nippori.Controls;

namespace Nippori.Pages
{
    /// <summary>
    /// Interaction logic for FlashcardPage.xaml
    /// </summary>
    public partial class FlashcardPage : Page
    {
        #region .: Private Variables :.

        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        #endregion

        #region .: Constructor :.

        public FlashcardPage()
        {
            InitializeComponent();

            CultureInfo japanCulture = new CultureInfo("ja-JP");
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            speechSynthesizer.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, japanCulture);

            DataContext = App.FlashCardsVM;
        }

        #endregion

        #region .: Private Methods :.

        /// <summary>
        /// Opens the given URL in default web browser.
        /// </summary>
        /// <param name="url">URL to open in the browser.</param>
        private void OpenWebBrowser(string url)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                };
                Process.Start(info);
            }
            catch (Win32Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region .: Event Handlers :.

        #region .: VocableField :.

        private void VocableField_CharacterMouseDown(object sender, EventArgs e)
        {
            VocableField vf = (VocableField)sender;

            string url = $"https://jisho.org/search/{vf.CharacterUnderCursor}%20%23kanji";
            OpenWebBrowser(url);
        }

        private void VocableField_ButtonClick(object sender, ButtonClickEventArgs e)
        {
            string url;
            VocableField vocableField = (VocableField)sender;

            switch (e.Command)
            {
                case ButtonCommands.Speak:
                    speechSynthesizer.Speak(vocableField.Text);
                    break;
                case ButtonCommands.DeepL:
                    url = $"https://www.deepl.com/translator#ja/en/{vocableField.Text}";
                    OpenWebBrowser(url);
                    break;
                case ButtonCommands.GoogleImages:
                    url = $"https://www.google.com/search?tbm=isch&q={vocableField.Text}";
                    OpenWebBrowser(url);
                    break;
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            switch (button.Tag)
            {
                case "EnableAll":
                    App.FlashCardsVM.EnableAllVocables();
                    break;
            }
        }

        private void ToggleButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // do not pass the key event to the button, it would change its IsChecked property
            e.Handled = true;

            switch (e.Key)
            {
                case Key.Space:
                    App.FlashCardsVM.MoveToNextVocable();
                    break;
                case Key.Enter:
                    App.FlashCardsVM.DisableCurrentVocable();
                    break;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            Clipboard.SetText(textBlock.Text);
        }

        #endregion

    }
}
