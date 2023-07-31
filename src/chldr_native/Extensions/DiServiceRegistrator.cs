using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Enums;
using chldr_utils.Services;
using chldr_data.local.Services;
using Blazored.Modal;

namespace chldr_native.Extensions
{
    internal static class DiServiceRegistrator
    {
        internal static MauiAppBuilder RegisterNativeAppServices(this MauiAppBuilder appBuilder)
        {
            ServiceRegistrator.RegisterCommonServices(appBuilder.Services);

            appBuilder.Services.AddMauiBlazorWebView();
            appBuilder.Services.AddBlazorWebViewDeveloperTools();

            appBuilder.Services.AddLocalization();
            appBuilder.Services.AddBlazoredModal();

            // Data    
            appBuilder.Services.AddScoped<IDataProvider, RealmDataProvider>();
            appBuilder.Services.AddScoped<SyncService>();

            appBuilder.Services.AddSingleton(x => new EnvironmentService(CurrentPlatform, IsDevelopment));

            return appBuilder;
        }

        private static Platforms CurrentPlatform
        {
            get
            {

#if ANDROID
            return Platforms.Android;
#elif IOS
            return Platforms.IOS;
#elif WINDOWS
            return Platforms.Windows;
#elif MACCATALYST
                return Platforms.MacCatalyst;
#endif
            }
        }

        private static bool IsDevelopment
        {
            get
            {
#if RELEASE
                return false;
#else
                return true;
#endif
            }
        }
    }
}
