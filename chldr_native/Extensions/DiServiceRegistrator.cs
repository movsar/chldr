using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
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
using Blazored.Modal;

namespace chldr_native.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static MauiAppBuilder RegisterValidators(this MauiAppBuilder appBuilder)
        {
            appBuilder.Services.AddScoped<EntryValidator>();
            appBuilder.Services.AddScoped<TranslationValidator>();
            appBuilder.Services.AddScoped<UserInfoValidator>();
            appBuilder.Services.AddScoped<IValidator<UserDto>, UserInfoValidator>();

            return appBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder appBuilder)
        {
            // Data
            appBuilder.Services.AddSingleton<ServiceLocator>();
            appBuilder.Services.AddSingleton<IGraphQLRequestSender, GraphQLRequestSender>();
            appBuilder.Services.AddScoped<AuthService>();
            appBuilder.Services.AddScoped<IDataProvider, RealmDataProvider>();

            appBuilder.Services.AddScoped<UserService>();
            appBuilder.Services.AddScoped<ISearchService, SearchService>();
            appBuilder.Services.AddScoped<SyncService>();

            // Shared
            appBuilder.Services.AddScoped<ContentStore>();
            appBuilder.Services.AddScoped<UserStore>();
            appBuilder.Services.AddScoped<JsInterop>();
            appBuilder.Services.AddScoped<LocalStorageService>();

            appBuilder.Services.AddBlazoredModal();
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

            var isDevelopment = true;
#if RELEASE
            isDevelopment = false;
#endif

            appBuilder.Services.AddSingleton(x => new EnvironmentService(platform, isDevelopment));
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
