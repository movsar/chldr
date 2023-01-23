using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_shared;
using chldr_shared.Dto;
using chldr_shared.Enums;
using chldr_shared.Services;
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
            appBuilder.Services.AddScoped<UserInfoValidator>();
            appBuilder.Services.AddScoped<IValidator<UserInfoDto>, UserInfoValidator>();

            return appBuilder;
        }
        public static WebApplicationBuilder RegisterAppServices(this WebApplicationBuilder appBuilder)
        {
            // Data
            appBuilder.Services.AddSingleton<IDataAccess, DataAccess>();

            // Shared
            appBuilder.Services.AddSingleton<ContentStore>();
            appBuilder.Services.AddSingleton<UserStore>();
            appBuilder.Services.AddScoped<JsInterop>();
            appBuilder.Services.AddScoped<EmailService>();
            appBuilder.Services.AddSingleton(new EnvironmentService(Platforms.Web));

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
