using CoreOS = Codeland.Core.OS;
using Core.Lib.OS;
using MAUIApp.OS;

namespace MAUIApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            CoreOS.DependencyService.Register<NavigationService, INavigationService>(CoreOS.DependencyService.ServiceLifetime.Singleton);
            CoreOS.DependencyService.Register<SettingsStorage, ISettingsStorage>();

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}