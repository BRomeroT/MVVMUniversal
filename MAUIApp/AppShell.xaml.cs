using CoreOS = Codeland.Core.OS;
using MAUIApp.OS;
using Core.Lib.OS;

namespace MAUIApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Fix: Use 'this' instead of 'Current' and 'MainPage'
            (CoreOS.DependencyService.Get<INavigationService>() as NavigationService).Navigation = this.Navigation;
        }
    }
}
