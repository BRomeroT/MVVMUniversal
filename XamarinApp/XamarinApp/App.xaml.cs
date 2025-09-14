using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApp.Views.OS;
using OS = Codeland.Core.OS;
using Core.Lib.OS;

namespace XamarinApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            OS.DependencyService.Register<NavigationService, INavigationService>(OS.DependencyService.ServiceLifetime.Singleton);
            OS.DependencyService.Register<SettingsStorage, ISettingsStorage>();

            MainPage = new NavigationPage(new MainPage());
            (OS.DependencyService.Get<INavigationService>() as NavigationService).Navigation = Current.MainPage.Navigation;
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
