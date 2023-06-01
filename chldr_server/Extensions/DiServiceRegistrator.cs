using chldr_data.Interfaces;
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
using chldr_data.Readers;
using chldr_data.ChangeRequests;

namespace chldr_blazor.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static WebApplicationBuilder RegisterValidators(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddSingleton<WordValidator>();
            appBuilder.Services.AddSingleton<TranslationValidator>();
            appBuilder.Services.AddSingleton<UserInfoValidator>();
            appBuilder.Services.AddSingleton<IValidator<UserDto>, UserInfoValidator>();

            return appBuilder;
        }
        public static WebApplicationBuilder RegisterAppServices(this WebApplicationBuilder appBuilder)
        {
            // Data
            appBuilder.Services.AddSingleton<ServiceLocator>();
            appBuilder.Services.AddScoped<UserService>();
            appBuilder.Services.AddSingleton<IGraphQLRequestSender, GraphQLRequestSender>();
            appBuilder.Services.AddSingleton<AuthService>();
            appBuilder.Services.AddSingleton<IDataSourceService, RealmDataSource>();
            appBuilder.Services.AddSingleton<ILocalDbReader, LocalDbReader>();
            appBuilder.Services.AddScoped<SyncService>();
            appBuilder.Services.AddScoped<WordQueries>();
            appBuilder.Services.AddScoped<LanguageQueries>();
            appBuilder.Services.AddScoped<PhraseQueries>();
            appBuilder.Services.AddScoped<SourceQueries>();
            appBuilder.Services.AddScoped<WordChangeRequests>();
            appBuilder.Services.AddScoped<SearchService>();


            // Shared
            appBuilder.Services.AddScoped<ContentStore>();
            appBuilder.Services.AddScoped<UserStore>();
            appBuilder.Services.AddScoped<JsInterop>();
            appBuilder.Services.AddScoped<LocalStorageService>();
            appBuilder.Services.AddSingleton<EmailService>();
            appBuilder.Services.AddSingleton(x => new EnvironmentService(Platforms.Web));
            appBuilder.Services.AddSingleton(x => new FileService(AppContext.BaseDirectory));

            // Utils
            appBuilder.Services.AddSingleton<ExceptionHandler>();
            appBuilder.Services.AddSingleton<CultureService>();
            appBuilder.Services.AddSingleton<NetworkService>();

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
