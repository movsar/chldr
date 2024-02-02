using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Services;
using chldr_data.Validators;
using chldr_shared.Validators;
using FluentValidation;
using chldr_utils.Services;
using chldr_data.Interfaces;
using chldr_app.Stores;
using Microsoft.Extensions.DependencyInjection;
using chldr_data.realm.Interfaces;
using chldr_data.realm.Services;

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

            services.AddSingleton<ISyncService, SyncService>();
            services.AddSingleton<IDataProvider, RealmDataProvider>();
            services.AddSingleton<ContentStore>();
        }
    }
}
