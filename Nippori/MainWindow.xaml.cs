using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using Nippori.ViewModel;

namespace Nippori
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region .: Properties :.

        public MainWindowViewModel MainWindowVM { get; private set; } = new MainWindowViewModel();

        #endregion

        #region .: Constructor :.

        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.FlashCardsVM;

            if (App.Args != null)
            {
                App.FlashCardsVM.LoadVocablesFromXmlFile(App.Args[0]);
                App.FlashCardsVM.StartTraining();
            }
        }

        #endregion

        #region .: Event Handlers :.

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                FilterIndex = 1,
                Multiselect = false,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                App.FlashCardsVM.LoadVocablesFromXmlFile(openFileDialog.FileName);
                App.FlashCardsVM.StartTraining();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Tag is string tag)
            {
                switch (tag)
                {
                    case "EnableAll":
                        App.FlashCardsVM.EnableAllVocables();
                        break;
                }
            }
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            App.FlashCardsVM.Test();
        }

        private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            App.FlashCardsVM.RestartTrainingIfSetupChanged();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            
            //switch (menuItem.Tag)
            //{
            //}
        }

        private void RibbonMenuButton_DropDownClosed(object sender, EventArgs e)
        {
            App.FlashCardsVM.RestartTrainingIfSetupChanged();
        }

        #endregion

    }
}
