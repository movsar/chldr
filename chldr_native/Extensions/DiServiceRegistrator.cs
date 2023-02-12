﻿using chldr_data.Dto;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_data.Validators;
using chldr_shared.Enums;
using chldr_shared.Services;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_ui;
using chldr_ui.ViewModels;
using chldr_utils;
using chldr_utils.Services;
using FluentValidation;

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
            appBuilder.Services.AddScoped<UserService>();

            appBuilder.Services.AddScoped<IRealmService, SyncedRealmService>();
            appBuilder.Services.AddScoped<IRealmService, OfflineRealmService>();
            appBuilder.Services.AddScoped<IDataAccess, OfflineDataAccess>();
            appBuilder.Services.AddScoped<IDataAccess, SyncedDataAccess>();

            appBuilder.Services.AddScoped<IDataAccessFactory, DataAccessFactory>();
            appBuilder.Services.AddScoped<IRealmServiceFactory, RealmServiceFactory>();

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

            // Repositories
            appBuilder.Services.AddTransient<EntriesRepository<EntryModel>>();
            appBuilder.Services.AddTransient<WordsRepository>();
            appBuilder.Services.AddTransient<LanguagesRepository>();
            appBuilder.Services.AddTransient<PhrasesRepository>();
            appBuilder.Services.AddTransient<SourcesRepository>();

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
