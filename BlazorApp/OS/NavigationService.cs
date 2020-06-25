using Core.Lib.OS;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.OS
{
    public class NavigationService : INavigationService
    {
        internal NavigationManager NavigationManager { get; set; }
        string currentPage, previousPage;

        public Task GoBack()
        {
            if (!string.IsNullOrEmpty(previousPage))
                NavigationManager.NavigateTo(previousPage);
            return Task.CompletedTask;
        }

        public Task Home()
        {
            previousPage = string.Empty;currentPage = "/";
            NavigationManager.NavigateTo("/");
            return Task.CompletedTask;
        }

        public void NavigatePop()
        {
            throw new NotImplementedException();
        }

        public Task NavigateTo(string pageKey)
        {
            previousPage = currentPage;
            currentPage = pageKey switch
            {
                PagesKeys.Login => "/",
                PagesKeys.Crud => "Crud",
                PagesKeys.Other => "Other",
                _ => "Login"
            };
            NavigationManager.NavigateTo(currentPage);
            return Task.CompletedTask;
        }

        public Task NavigateTo(string pageKey, params object[] parameter)
        {
            previousPage = currentPage;
            //For demo only, concant for each page required parameters 
            currentPage = pageKey switch
            {
                PagesKeys.Login => $"/?param1={parameter[0]}",
                PagesKeys.Crud => $"Crud?param1={parameter[0]}",
                _ => "Login"
            };
            NavigationManager.NavigateTo(currentPage);
            return Task.CompletedTask;
        }

        public void NavigateToUrl(string url) => NavigationManager.NavigateTo(url);

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
