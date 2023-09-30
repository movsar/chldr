using chldr_api.GraphQL.MutationResolvers;
using chldr_api.GraphQL.MutationServices;
using chldr_data.Interfaces;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

namespace chldr_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("SqlContext");
            var signingSecret = builder.Configuration.GetValue<string>("ApiJwtSigningKey");
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(signingSecret))
            {
                throw new Exception("Connection string is not set");
            }
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingSecret));

            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<FileService>();
            builder.Services.AddScoped<ExceptionHandler>();

            builder.Services.AddTransient<UserResolver>();
            builder.Services.AddTransient<EntryResolver>();

            builder.Services.AddLocalization();

            // SQL Services **************************************************************
            builder.Services.AddDbContext<SqlContext>(options => options
                         .UseMySQL(connectionString, b => b.MigrationsAssembly("chldr_api")), ServiceLifetime.Transient);

            builder.Services.AddTransient<IDataProvider, SqlDataProvider>();
            builder.Services
                .AddDefaultIdentity<SqlUser>()
                .AddEntityFrameworkStores<SqlContext>();

            builder.Services.Configure<IdentityOptions>(options =>
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
            // *****************************************************************************

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");


            builder.Services.AddAuthentication(options =>
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

            builder.Services.AddAuthorization();

            builder.Services.AddTransient<Mutation>();
            builder.Services.AddTransient<Query>();
            builder.Services.AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddProjections()
                .AddFiltering()
                .AddSorting();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGraphQL("/graphql");
            app.Run();
        }
    }
}