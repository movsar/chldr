using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_shared;
using chldr_shared.Pages;
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
            mauiAppBuilder.Services.AddScoped<RegistrationValidator>();
            mauiAppBuilder.Services.AddScoped<IValidator<RegistrationPageViewModel>, RegistrationValidator>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IDataAccess, DataAccess>();
            mauiAppBuilder.Services.AddSingleton<ContentStore>();
            mauiAppBuilder.Services.AddScoped<JsInterop>();

            mauiAppBuilder.Services.AddLocalization();
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
