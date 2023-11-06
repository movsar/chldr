using Blazored.Modal;
using chldr_data.realm.Services;
using chldr_data.Interfaces;
using chldr_shared.Enums;
using chldr_utils.Services;
using chldr_web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace chldr_web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            
            ServiceRegistrator.RegisterCommonServices(builder.Services);
            builder.Services.AddBlazoredModal();
            builder.Services.AddTransient(x => new EnvironmentService(Platforms.Web, false));
            
            builder.Services.AddTransient<IDataProvider, RealmDataProvider>();
            
            // Localization
            builder.Services.AddLocalization();

            // Set the culture
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");

            var app = builder.Build();
            await app.RunAsync();
        }
    }
}