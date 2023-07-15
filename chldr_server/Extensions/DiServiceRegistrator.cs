using Blazored.Modal;
using chldr_data.Interfaces;
using chldr_data.local.Services;
using chldr_shared.Enums;
using chldr_utils.Services;

namespace chldr_blazor.Extensions
{
    internal static class DiServiceRegistrator
    {
        internal static WebApplicationBuilder RegisterWebAppServices(this WebApplicationBuilder appBuilder)
        {
            ServiceRegistrator.RegisterCommonServices(appBuilder.Services);
          
            appBuilder.Services.AddRazorPages();
            appBuilder.Services.AddServerSideBlazor();
            appBuilder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
           
            appBuilder.Services.AddLocalization();
            appBuilder.Services.AddBlazoredModal();

            appBuilder.Services.AddScoped<IDataProvider, RealmDataProvider>();
            appBuilder.Services.AddScoped<ISearchService, SearchService>();
            appBuilder.Services.AddScoped<SyncService>();

            appBuilder.Services.AddSingleton(x => new EnvironmentService(Platforms.Web, appBuilder.Environment.IsDevelopment()));
            
            return appBuilder;
        }
    }
}
