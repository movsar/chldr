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
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            // Data
            mauiAppBuilder.Services.AddScoped<IDataAccess, DataAccess>();
            mauiAppBuilder.Services.AddScoped<RealmService>();

            // Shared
            mauiAppBuilder.Services.AddScoped<ContentStore>();
            mauiAppBuilder.Services.AddScoped<UserStore>();
            mauiAppBuilder.Services.AddScoped<JsInterop>();
            mauiAppBuilder.Services.AddScoped<EmailService>();
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
            mauiAppBuilder.Services.AddScoped(x => new EnvironmentService(platform));
            mauiAppBuilder.Services.AddScoped(x => new FileService(AppContext.BaseDirectory));
            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddScoped<MainPageViewModel>();
            mauiAppBuilder.Services.AddScoped<LoginPageViewModel>();
            mauiAppBuilder.Services.AddScoped<RegistrationPageViewModel>();
            return mauiAppBuilder;
        }
    }
}
