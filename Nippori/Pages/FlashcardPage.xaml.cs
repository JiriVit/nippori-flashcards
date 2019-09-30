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

namespace Nippori.Pages
{
    /// <summary>
    /// Interaction logic for FlashcardPage.xaml
    /// </summary>
    public partial class FlashcardPage : Page
    {
        public FlashcardPage()
        {
            InitializeComponent();
            DataContext = App.MyViewModel;
        }
    }
}
