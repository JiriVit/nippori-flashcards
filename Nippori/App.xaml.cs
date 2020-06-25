using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Nippori.ViewModel;
using Nippori.Japanese;

namespace Nippori
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region .: Properties :.

        public static MainWindowViewModel MainWindowVM { get; private set; } = new MainWindowViewModel();
        public static FlashCardsViewModel FlashCardsVM { get; private set; } = new FlashCardsViewModel();

        public static string[] Args;

        #endregion

        #region .: Event Handlers :.

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                Args = e.Args;
            }

            JlptUtils.Init();
        }

        #endregion

    }
}
