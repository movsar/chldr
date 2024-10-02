using domain.DatabaseObjects.Dtos;
using domain.Services;
using domain.Validators;
using chldr_shared.Validators;
using FluentValidation;
using chldr_utils.Services;
using domain.Interfaces;
using chldr_app.Stores;
using Microsoft.Extensions.DependencyInjection;
using domain.Models;
using chldr_application.Services;
using domain.Enums;

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
            services.AddScoped<ContentStore>();

            // Web
            services.AddScoped<CultureService>();
            services.AddScoped<IExceptionHandler, ExceptionHandler>();
        }
    }
}
