using chldr_data.Services;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_shared.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace chldr_web.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static WebAssemblyHostBuilder RegisterValidators(this WebAssemblyHostBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<RegistrationValidator>();
            appBuilder.Services.AddScoped<IValidator<RegistrationPageViewModel>, RegistrationValidator>();

            return appBuilder;
        }
        public static WebAssemblyHostBuilder RegisterAppServices(this WebAssemblyHostBuilder appBuilder)
        {
            appBuilder.Services.AddSingleton<DataAccess>();
            appBuilder.Services.AddSingleton<ContentStore>();
            appBuilder.Services.AddLocalization();
            return appBuilder;
        }
        public static WebAssemblyHostBuilder RegisterViewModels(this WebAssemblyHostBuilder appBuilder)
        {
            appBuilder.Services.AddSingleton<MainPageViewModel>();
            appBuilder.Services.AddSingleton<LoginPageViewModel>();
            appBuilder.Services.AddSingleton<RegistrationPageViewModel>();
            return appBuilder;
        }
    }
}
