using Blazored.Modal;
using chldr_app.Services;
using core.Enums;
using core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace chldr_blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Add services to the container.          
            ServiceRegistrator.RegisterCommonServices(builder.Services);

            builder.Services.AddBlazoredModal();

            builder.Services.AddTransient(x => new EnvironmentService(Platforms.Web, true));

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");

            await builder.Build().RunAsync();
        }
    }
}
