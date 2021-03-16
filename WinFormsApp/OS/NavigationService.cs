using Core.Lib.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp.OS
{
    internal class NavigationService : INavigationService
    {
        public Task GoBack()
        {
            throw new NotImplementedException();
        }

        public Task Home()
        {
            throw new NotImplementedException();
        }

        public void NavigatePop()
        {
            throw new NotImplementedException();
        }

        public Task NavigateTo(string pageKey)
        {
            throw new NotImplementedException();
        }

        public Task NavigateTo(string pageKey, params object[] parameter)
        {
            throw new NotImplementedException();
        }

        public void NavigateToUrl(string url)
        {
            throw new NotImplementedException();
        }

        public Task PopModal()
        {
            throw new NotImplementedException();
        }

        public Task PushModal(string pageKey)
        {
            throw new NotImplementedException();
        }
    }
}
