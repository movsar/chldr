using chldr_data.Dto;
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
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder appBuilder)
        {
            // Data
            appBuilder.Services.AddSingleton<ServiceLocator>();
            appBuilder.Services.AddSingleton<UserService>();
            appBuilder.Services.AddSingleton<IRealmService, SyncedRealmService>();
            appBuilder.Services.AddSingleton<IRealmService, OfflineRealmService>();
            appBuilder.Services.AddSingleton<IRealmServiceFactory, RealmServiceFactory>();
            appBuilder.Services.AddSingleton<IDataAccess, DataAccess>();

            // Shared
            appBuilder.Services.AddSingleton<ContentStore>();
            appBuilder.Services.AddSingleton<UserStore>();
            appBuilder.Services.AddScoped<JsInterop>();
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

            // Repositories
            appBuilder.Services.AddSingleton<EntriesRepository<EntryModel>>();
            appBuilder.Services.AddSingleton<WordsRepository>();
            appBuilder.Services.AddSingleton<LanguagesRepository>();
            appBuilder.Services.AddSingleton<PhrasesRepository>();
            appBuilder.Services.AddSingleton<SourcesRepository>();
            appBuilder.Services.AddSingleton<UsersRepository>();

            return appBuilder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<MainPageViewModel>();
            appBuilder.Services.AddScoped<LoginPageViewModel>();
            appBuilder.Services.AddScoped<RegistrationPageViewModel>();
            appBuilder.Services.AddScoped<SearchResultsViewModel>();
            appBuilder.Services.AddTransient<UsersRepository>();

            return appBuilder;
        }
    }
}
