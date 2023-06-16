using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Services;
using chldr_data.Validators;
using chldr_shared.Enums;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_ui;
using chldr_ui.ViewModels;
using chldr_utils.Services;
using chldr_data.local.Services;
using Blazored.Modal;
namespace chldr_native.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static MauiAppBuilder RegisterNativeAppServices(this MauiAppBuilder appBuilder)
        {
            ServiceRegistrator.RegisterCommonServices(appBuilder.Services);

            // Data    
            appBuilder.Services.AddScoped<IDataProvider, RealmDataProvider>();
            appBuilder.Services.AddScoped<ISearchService, SearchService>();
            appBuilder.Services.AddScoped<SyncService>();

            appBuilder.Services.AddBlazoredModal();

            var platform = Platforms.Web;
#if ANDROID
            platform = Platforms.Android;
#elif IOS
            platform = Platforms.IOS;
#elif WINDOWS
            platform = Platforms.Windows;
#elif MACCATALYST
            platform = Platforms.MacCatalyst;
#endif

            var isDevelopment = true;
#if RELEASE
            isDevelopment = false;
#endif

            appBuilder.Services.AddSingleton(x => new EnvironmentService(platform, isDevelopment));
            
            return appBuilder;
        }
    }
}
