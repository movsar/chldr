using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_ui;
using chldr_shared.Enums;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_ui.ViewModels;
using FluentValidation;
using chldr_shared.Dto;
using chldr_shared.Services;
using System.Reflection.PortableExecutable;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_native.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static MauiAppBuilder RegisterValidators(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddScoped<WordValidator>();
            mauiAppBuilder.Services.AddScoped<TranslationValidator>();
            mauiAppBuilder.Services.AddScoped<UserInfoValidator>();
            mauiAppBuilder.Services.AddScoped<IValidator<UserInfoDto>, UserInfoValidator>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder appBuilder)
        {
            // Data
            appBuilder.Services.AddScoped<IDataAccess, DataAccess>();
            appBuilder.Services.AddScoped<RealmService>();

            // Shared
            appBuilder.Services.AddScoped<ContentStore>();
            appBuilder.Services.AddScoped<UserStore>();
            appBuilder.Services.AddScoped<JsInterop>();
            appBuilder.Services.AddScoped<EmailService>();
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
            appBuilder.Services.AddScoped(x => new EnvironmentService(platform));
            appBuilder.Services.AddScoped(x => new FileService(AppContext.BaseDirectory));

            // Utils
            appBuilder.Services.AddScoped<ExceptionHandler>();
            appBuilder.Services.AddScoped<NetworkService>();

            return appBuilder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddScoped<MainPageViewModel>();
            mauiAppBuilder.Services.AddScoped<LoginPageViewModel>();
            mauiAppBuilder.Services.AddScoped<RegistrationPageViewModel>();
            mauiAppBuilder.Services.AddScoped<SearchResultsViewModel>();

            return mauiAppBuilder;
        }
    }
}
