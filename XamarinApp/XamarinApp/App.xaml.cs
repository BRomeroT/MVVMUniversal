using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApp.Views.OS;

namespace XamarinApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Sysne.Core.OS.DependencyService.Register<SettingsStorage, Core.Lib.OS.ISettingsStorage>();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
