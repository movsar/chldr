using Blazored.Modal;
using chldr_app.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace chldr_blazor_wa
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddLocalization();
            builder.Services.AddBlazoredModal(); 
            ServiceRegistrator.RegisterCommonServices(builder.Services);
          
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after"); 

            builder.Services.AddScoped(sp => new HttpClient { 
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
            });

            await builder.Build().RunAsync();
        }
    }
}
