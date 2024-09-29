using api_server.GraphQL.MutationResolvers;
using api_server.GraphQL.MutationServices;
using domain.Interfaces;
using domain.Models;
using domain.Services;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using sql_dl.SqlEntities;
using sql_dl.Services;
using sql_dl;

namespace api_server
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlContext");
            var signingSecret = configuration.GetValue<string>("ApiJwtSigningKey");
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(signingSecret))
            {
                throw new Exception("Connection string is not set");
            }

            services.AddScoped<EmailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IExceptionHandler, ExceptionHandler>();

            services.AddTransient<UserResolver>();
            services.AddTransient<EntryResolver>();

            // Add your localization service if needed

            // SQL Services
            services.AddDbContext<SqlContext>(options => options
                .UseMySQL(connectionString, b => b.MigrationsAssembly("api_server")), ServiceLifetime.Transient);

            services.AddTransient<IDataProvider, SqlDataProvider>();
            services.AddDefaultIdentity<SqlUser>()
                .AddEntityFrameworkStores<SqlContext>();

            // Configure Identity options if needed
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.User.RequireUniqueEmail = true;
            });

            // Set culture
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");

            // Add authentication
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingSecret));
            services.AddHttpContextAccessor();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Dosham",
                    ValidAudience = "dosham.app",
                    IssuerSigningKey = signingKey
                };
            });

            // Add authorization if needed
            services.AddAuthorization();

            // Add GraphQL services
            services.AddTransient<Mutation>();
            services.AddTransient<Query>();
            services.AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddProjections()
                .AddFiltering()
                .AddSorting();
        }
    }
}
