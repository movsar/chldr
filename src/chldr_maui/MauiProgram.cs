using chldr_data.Interfaces;
using chldr_data.realm.Services;
using dosham.Enums;
using dosham.Pages;
using dosham.Services;
using dosham.Stores;
using dosham.ViewModels;
using Microsoft.Extensions.Logging;

namespace dosham
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            ServiceRegistrator.RegisterCommonServices(builder.Services);

            builder.Services.AddLocalization();

            // Data    
            builder.Services.AddSingleton<IEnvironmentService>(x => new EnvironmentService(CurrentPlatform, IsDevelopment));
            
            builder.Services.AddSingleton<IDataProvider, RealmDataProvider>();
            builder.Services.AddSingleton<SyncService>();
            builder.Services.AddSingleton<ContentStore>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<IndexPageViewModel>();

            builder.Services.AddTransient<EntriesViewModel>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
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
