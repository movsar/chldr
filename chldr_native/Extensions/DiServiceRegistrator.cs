﻿using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_data.Validators;
using chldr_shared.Enums;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_ui;
using chldr_ui.ViewModels;
using chldr_utils;
using chldr_utils.Services;
using FluentValidation;
using chldr_utils.Interfaces;
using chldr_data.local.Services;

namespace chldr_native.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static MauiAppBuilder RegisterValidators(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddScoped<WordValidator>();
            mauiAppBuilder.Services.AddScoped<TranslationValidator>();
            mauiAppBuilder.Services.AddScoped<UserInfoValidator>();
            mauiAppBuilder.Services.AddScoped<IValidator<UserDto>, UserInfoValidator>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder appBuilder)
        {
            // Data
            appBuilder.Services.AddSingleton<ServiceLocator>();
            appBuilder.Services.AddSingleton<IGraphQLRequestSender, GraphQLRequestSender>();
            appBuilder.Services.AddSingleton<AuthService>();
            appBuilder.Services.AddSingleton<IDataProvider, RealmDataProvider>();
            
            appBuilder.Services.AddScoped<UserService>();
            appBuilder.Services.AddScoped<ISearchService, SearchService>();
            //appBuilder.Services.AddScoped<SyncService>();
            
            // Shared
            appBuilder.Services.AddScoped<ContentStore>();
            appBuilder.Services.AddScoped<UserStore>();
            appBuilder.Services.AddScoped<JsInterop>();
            appBuilder.Services.AddScoped<LocalStorageService>();

            appBuilder.Services.AddSingleton<EmailService>();

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
            appBuilder.Services.AddSingleton(x => new EnvironmentService(platform));
            appBuilder.Services.AddSingleton(x => new FileService(AppContext.BaseDirectory));

            // Utils
            appBuilder.Services.AddSingleton<ExceptionHandler>();
            appBuilder.Services.AddSingleton<CultureService>();
            appBuilder.Services.AddSingleton<NetworkService>();

            return appBuilder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<MainPageViewModel>();
            appBuilder.Services.AddScoped<LoginPageViewModel>();
            appBuilder.Services.AddScoped<RegistrationPageViewModel>();
            appBuilder.Services.AddScoped<SearchResultsViewModel>();

            return appBuilder;
        }
    }
}
