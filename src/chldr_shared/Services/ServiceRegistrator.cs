using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Services;
using chldr_data.Validators;
using chldr_shared.Stores;
using chldr_shared.Validators;
using FluentValidation;
using chldr_utils.Interfaces;
using chldr_shared;
using Microsoft.Extensions.DependencyInjection;
using chldr_shared.Services;
using chldr_data.Interfaces;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using Microsoft.AspNetCore.Identity;

namespace chldr_utils.Services
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
            services.AddScoped<RequestService>();

            services.AddScoped<UserService>();

            // Shared
            services.AddScoped<ContentStore>();

            services.AddScoped<EntryService>();
            services.AddScoped<SourceService>();
            services.AddScoped<UserStore>();

            services.AddScoped<JsInteropService>();
            services.AddScoped<LocalStorageService>();

            services.AddScoped<EmailService>();
            services.AddScoped<FileService>();

            // Utils
            services.AddScoped<ExceptionHandler>();
            services.AddScoped<CultureService>();
        }
    }
}
