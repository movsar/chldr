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

namespace chldr_server.Extensions
{
    internal static class DiServiceRegistrator
    {
        public static WebApplicationBuilder RegisterValidators(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddSingleton<WordValidator>();
            appBuilder.Services.AddSingleton<TranslationValidator>();
            appBuilder.Services.AddSingleton<UserInfoValidator>();
            appBuilder.Services.AddSingleton<IValidator<UserInfoDto>, UserInfoValidator>();

            return appBuilder;
        }
        public static WebApplicationBuilder RegisterAppServices(this WebApplicationBuilder appBuilder)
        {
            // Data
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
            appBuilder.Services.AddSingleton(x => new EnvironmentService(Platforms.Web));
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
        public static WebApplicationBuilder RegisterViewModels(this WebApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddSingleton<MainPageViewModel>();
            appBuilder.Services.AddSingleton<LoginPageViewModel>();
            appBuilder.Services.AddSingleton<RegistrationPageViewModel>();

            return appBuilder;
        }
    }
}
