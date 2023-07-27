using chldr_api.GraphQL.MutationServices;
using chldr_data.Interfaces;
using chldr_data.remote.Services;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            builder.Services.AddTransient<IDataProvider, SqlDataProvider>();

            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<FileService>();
            builder.Services.AddScoped<ExceptionHandler>();

            builder.Services.AddScoped<PasswordResetResolver>();
            builder.Services.AddScoped<UpdatePasswordResolver>();
            builder.Services.AddScoped<RegisterUserResolver>();
            builder.Services.AddScoped<ConfirmEmailResolver>();
            builder.Services.AddScoped<LoginResolver>();


            builder.Services.AddLocalization();

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingSecret));

            builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidIssuer = "https://auth.chillicream.com",
                        ValidAudience = "https://graphql.chillicream.com",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey
                    };
            });

            builder.Services.AddAuthorization();

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
                app.UseHttpsRedirection();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGraphQL("/graphql");
            app.Run();
        }
    }
}