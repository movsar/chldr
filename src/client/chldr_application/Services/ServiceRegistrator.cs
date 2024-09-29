using core.DatabaseObjects.Dtos;
using core.Services;
using core.Validators;
using chldr_shared.Validators;
using FluentValidation;
using chldr_utils.Services;
using core.Interfaces;
using chldr_app.Stores;
using Microsoft.Extensions.DependencyInjection;
using chldr_application.ApiDataProvider.Services;
using core.Enums;
using core.Models;
using chldr_application.Services;

namespace chldr_app.Services
{
    public static class ServiceRegistrator
    {

        public static void RegisterCommonServices(IServiceCollection services)
        {
            RegisterValidators(services);
            RegisterAppServices(services);
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            services.AddScoped<EntryValidator>();
            services.AddScoped<TranslationValidator>();
            services.AddScoped<UserInfoValidator>();
            services.AddScoped<IValidator<UserDto>, UserInfoValidator>();

            return;
        }
        private static void RegisterAppServices(IServiceCollection services)
        {
            // Data
            services.AddScoped<IGraphQlClient, GraphQLClient>();
            services.AddScoped<IRequestService, RequestService>();

            services.AddScoped<UserService>();

            // Shared
            services.AddScoped<EntryCacheService>();
            services.AddScoped<EntryService>();
            services.AddScoped<SourceService>();
            services.AddScoped<UserStore>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<EmailService>();
            services.AddScoped<IDataProvider, ApiDataProvider>();
            services.AddScoped<ContentStore>();

            // Web
            services.AddScoped<CultureService>();
            services.AddScoped<IExceptionHandler, ExceptionHandler>();
            services.AddScoped<IEnvironmentService>(x => new EnvironmentService(Platforms.Web, true));
            services.AddScoped<ISettingsService, JsonFileSettingsService>();
            services.AddScoped<JsInteropService, JsInteropService>();
        }
    }
}
