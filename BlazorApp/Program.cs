using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorApp.OS;
using System.Net.Http.Json;
using Sysne.Core.OS;
using Core.Lib.OS;
using Microsoft.AspNetCore.Components;

namespace BlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            SettingsStorage.CurrentValues = await httpClient.GetFromJsonAsync<SettingsStorage.Values>("settings.json");

            DependencyService.Register<SettingsStorage, ISettingsStorage>();
            DependencyService.Register<NavigationService, INavigationService>(DependencyService.ServiceLifetime.Singleton);
            
            await builder.Build().RunAsync();
        }
    }
}
