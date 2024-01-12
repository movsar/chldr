﻿using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.realm.Services;
using chldr_data.Services;
using dosham.Services;
using dosham.Stores;
using dosham.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace dosham
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            ServiceRegistrator.RegisterCommonServices(builder.Services);

            builder.Services.AddLocalization();

            // Services
            builder.Services.AddSingleton<IEnvironmentService>(x => new EnvironmentService(CurrentPlatform, IsDevelopment));
            builder.Services.AddSingleton<SyncService>();
            builder.Services.AddSingleton<IDataProvider, RealmDataProvider>();
            builder.Services.AddSingleton<ContentStore>();
            builder.Services.AddSingleton(AudioManager.Current);

            // ViewModels
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddSingleton<IndexPageViewModel>();
            builder.Services.AddSingleton<AlphabetPageViewModel>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddTransient<EntriesViewModel>();
            builder.Services.AddTransient<RegistrationPageViewModel>();

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
