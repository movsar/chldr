﻿using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_ui;
using chldr_shared.Enums;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_ui.ViewModels;
using FluentValidation;
using chldr_shared.Dto;
using chldr_shared.Services;
using chldr_data.Search;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_server.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static WebApplicationBuilder RegisterValidators(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<WordValidator>();
            appBuilder.Services.AddScoped<TranslationValidator>();
            appBuilder.Services.AddScoped<UserInfoValidator>();
            appBuilder.Services.AddScoped<IValidator<UserInfoDto>, UserInfoValidator>();

            return appBuilder;
        }
        public static WebApplicationBuilder RegisterAppServices(this WebApplicationBuilder appBuilder)
        {
            // Data
            appBuilder.Services.AddScoped<UserService>();
            appBuilder.Services.AddScoped<IRealmService, OfflineRealmService>();
            appBuilder.Services.AddScoped<IDataAccess, OfflineDataAccess>();

            // Shared

            appBuilder.Services.AddScoped<ContentStore>();
            appBuilder.Services.AddScoped<UserStore>();
            appBuilder.Services.AddScoped<JsInterop>();
            appBuilder.Services.AddScoped<EmailService>();
            appBuilder.Services.AddScoped(x => new EnvironmentService(Platforms.Web));
            appBuilder.Services.AddScoped(x => new FileService(AppContext.BaseDirectory));

            // Utils
            appBuilder.Services.AddScoped<ExceptionHandler>();
            appBuilder.Services.AddScoped<NetworkService>();

            return appBuilder;
        }
        public static WebApplicationBuilder RegisterViewModels(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<MainPageViewModel>();
            appBuilder.Services.AddScoped<LoginPageViewModel>();
            appBuilder.Services.AddScoped<RegistrationPageViewModel>();



            return appBuilder;
        }
    }
}
