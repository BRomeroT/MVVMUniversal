using Core.Lib.OS;
using Sysne.Core.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp.OS;
using WinFormsApp.Views;

namespace WinFormsApp
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DependencyService.Register<NavigationService, INavigationService>(DependencyService.ServiceLifetime.Singleton);
            DependencyService.Register<SettingsStorage, ISettingsStorage>();


            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
