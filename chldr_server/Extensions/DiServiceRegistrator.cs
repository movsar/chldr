using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_shared.ViewModels;
using FluentValidation;

namespace chldr_server.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static WebApplicationBuilder RegisterValidators(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<RegistrationValidator>();
            appBuilder.Services.AddScoped<IValidator<RegistrationPageViewModel>, RegistrationValidator>();

            return appBuilder;
        }
        public static WebApplicationBuilder RegisterAppServices(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddSingleton<IDataAccess, DataAccess>();
            appBuilder.Services.AddSingleton<ContentStore>();
            appBuilder.Services.AddLocalization();
            return appBuilder;
        }
        public static WebApplicationBuilder RegisterViewModels(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddSingleton<MainPageViewModel>();
            appBuilder.Services.AddSingleton<LoginPageViewModel>();
            appBuilder.Services.AddSingleton<RegistrationPageViewModel>();
            return appBuilder;
        }
    }
}
