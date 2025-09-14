using Core.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Pages
{
    public partial class Index
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            (Sysne.Core.OS.DependencyService.Get<Core.Lib.OS.INavigationService>() as BlazorApp.OS.NavigationService)
                .NavigationManager = NavigationManager;
        }

        readonly LoginViewModel viewModel = new LoginViewModel();
        EditContext editContext = new EditContext(new object());
        private async Task Submit()
        {
            await viewModel.LoginCommand.ExecuteAsync();
            StateHasChanged();
        }
    }
}
