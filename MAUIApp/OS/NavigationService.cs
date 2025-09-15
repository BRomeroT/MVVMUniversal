using Core.Lib.OS;
using MAUIApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIApp.OS
{
    internal class NavigationService : INavigationService
    {
        internal INavigation Navigation { get; set; }

        public async Task GoBack() => await Navigation.PopAsync(true);

        public async Task Home() => await Navigation.PopToRootAsync(true);

        public async void NavigatePop() => await Navigation.PopAsync();

        public async Task NavigateTo(string pageKey)
        {
            if (pageKey == PagesKeys.Login)
            {
                await Navigation.PopToRootAsync(true);
                return;
            }

            var paginaPorNavegar = pageKey switch
            {
                PagesKeys.Crud => typeof(CrudPage),
                PagesKeys.Other => typeof(OtherPage),
                _ => typeof(MainPage)
            };

            var ultimaPagina = Navigation.NavigationStack.Where(p => p.GetType() == paginaPorNavegar).FirstOrDefault();
            if (ultimaPagina == null)
            {
                switch (pageKey)
                {
                    case PagesKeys.Login:
                        await Navigation.PopToRootAsync(true); break;
                    case PagesKeys.Crud:
                        await Navigation.PushAsync(new CrudPage(), true); break;
                    case PagesKeys.Other:
                        await Navigation.PushAsync(new OtherPage(), true); break;
                }
            }
        }

        public async Task NavigateTo(string pageKey, params object[] parameter)
        {
            switch (pageKey)
            {
                case PagesKeys.Login:
                    await Navigation.PushAsync(new MainPage()/*(parameter)*/, true); break;
                case PagesKeys.Crud:
                    await Navigation.PushAsync(new CrudPage()/*(parameter)*/, true); break;
            }
        }

        public async void NavigateToUrl(string url) => await Launcher.OpenAsync(new Uri(url));

        public Task PopModal() => throw new NotImplementedException();

        public Task PushModal(string pageKey) => null;
    }
}
