using chldr_shared.Enums;
using chldr_ui.ViewModels;
using chldr_utils.Services;

namespace chldr_blazor.Extensions
{
    internal static class DiServiceRegistrator
    {     
        public static WebApplicationBuilder RegisterWebAppServices(this WebApplicationBuilder appBuilder)
        {
            ServiceRegistrator.RegisterCommonServices(appBuilder.Services);

            //builder.Services.AddScoped<IDataProvider, SqlDataProvider>();
            //builder.Services.AddScoped<ISearchService, SqlSearchService>();

            appBuilder.Services.AddSingleton(x => new EnvironmentService(Platforms.Web, appBuilder.Environment.IsDevelopment()));
            
            return appBuilder;
        }
        public static WebApplicationBuilder RegisterViewModels(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<MainPageViewModel>();
            appBuilder.Services.AddScoped<LoginPageViewModel>();
            appBuilder.Services.AddScoped<RegistrationPageViewModel>();
            appBuilder.Services.AddScoped<SearchResultsViewModel>();

            return appBuilder;
        }
    }
}
