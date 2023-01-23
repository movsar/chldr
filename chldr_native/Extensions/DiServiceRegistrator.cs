using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_shared;
using chldr_shared.Dto;
using chldr_shared.Enums;
using chldr_shared.Pages;
using chldr_shared.Services;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_shared.ViewModels;
using FluentValidation;
using System.Reflection.PortableExecutable;

namespace chldr_native.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static MauiAppBuilder RegisterValidators(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddScoped<UserInfoValidator>();
            mauiAppBuilder.Services.AddScoped<IValidator<UserInfoDto>, UserInfoValidator>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            // Data
            mauiAppBuilder.Services.AddSingleton<IDataAccess, DataAccess>();

            // Shared
            mauiAppBuilder.Services.AddSingleton<ContentStore>();
            mauiAppBuilder.Services.AddSingleton<UserStore>();
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
            mauiAppBuilder.Services.AddScoped<EnvironmentService>(x => new EnvironmentService(platform));
            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<MainPageViewModel>();
            mauiAppBuilder.Services.AddSingleton<LoginPageViewModel>();
            mauiAppBuilder.Services.AddSingleton<RegistrationPageViewModel>();
            return mauiAppBuilder;
        }
    }
}
