using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Nippori.Pages
{
    /// <summary>
    /// Interaction logic for FlashcardPage.xaml
    /// </summary>
    public partial class FlashcardPage : Page
    {
        #region .: Constructor :.

        public FlashcardPage()
        {
            InitializeComponent();
            DataContext = App.FlashCardsVM;
        }

        #endregion

        #region .: Event Handlers :.

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
