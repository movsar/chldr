using chldr_shared.Enums;
using chldr_utils.Services;

namespace chldr_native
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.RegisterAppServices();

#if DEBUG
            //builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        internal static MauiAppBuilder RegisterAppServices(this MauiAppBuilder appBuilder)
        {
            //ServiceRegistrator.RegisterCommonServices(appBuilder.Services);

            appBuilder.Services.AddMauiBlazorWebView();
            appBuilder.Services.AddBlazorWebViewDeveloperTools();

            appBuilder.Services.AddLocalization();
            //appBuilder.Services.AddBlazoredModal();

            // Data    
            //appBuilder.Services.AddScoped<IDataProvider, RealmDataProvider>();
            //appBuilder.Services.AddScoped<SyncService>();

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