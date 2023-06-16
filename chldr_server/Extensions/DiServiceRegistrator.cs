using Blazored.Modal;
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

            //builder.Services.AddScoped<IDataProvider, SqlDataProvider>();
            //builder.Services.AddScoped<ISearchService, SqlSearchService>();

            appBuilder.Services.AddSingleton(x => new EnvironmentService(Platforms.Web, appBuilder.Environment.IsDevelopment()));
            
            return appBuilder;
        }
    }
}
