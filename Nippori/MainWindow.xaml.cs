using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                App.FlashCardsVM.OpenFile(App.Args[0]);
            }
        }

        #endregion

        #region .: Event Handlers :.

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    App.FlashCardsVM.Confirm();
                    break;
                case Key.Enter:
                    App.FlashCardsVM.Disable();
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
                App.FlashCardsVM.OpenFile(openFileDialog.FileName);
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            App.FlashCardsVM.Test();
        }

        private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            // TODO do this only if changes were made, not for each submenu closed
            App.FlashCardsVM.StartExam();
        }

        #endregion
    }
}
