using chldr_app.Services;
using core.Enums;
using core.Interfaces;
using core.Models;
using core.Services;
using chldr_utils.Services;
using chldr_application.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using chldr_application.Services;
using chldr_domain.Interfaces;
using dosham.Services;

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
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<ISettingsService, MauiSettingsService>();
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();

            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IExceptionHandler, ExceptionHandler>();
            builder.Services.AddScoped<CultureService>();

            // ViewModels
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddSingleton<IndexPageViewModel>();
            builder.Services.AddSingleton<AlphabetPageViewModel>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddTransient<EntriesViewModel>();
            builder.Services.AddTransient<RegistrationPageViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();

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
