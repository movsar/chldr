using core.DatabaseObjects.Dtos;
using core.Services;
using core.Validators;
using chldr_shared.Validators;
using FluentValidation;
using chldr_utils.Services;
using core.Interfaces;
using chldr_app.Stores;
using Microsoft.Extensions.DependencyInjection;
using chldr_domain.Interfaces;
using chldr_domain.Services;

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
            services.AddScoped<ContentStore>();

            services.AddScoped<EntryCacheService>();
            services.AddScoped<EntryService>();
            services.AddScoped<SourceService>();
            services.AddScoped<UserStore>();

            services.AddScoped<ISyncService, SyncService>();
            services.AddScoped<IDataProvider, RealmDataProvider>();
            services.AddScoped<ContentStore>();
        }
    }
}
