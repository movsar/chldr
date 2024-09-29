using Blazored.Modal;
using chldr_app.Services;
using chldr_application.Services;
using domain;
using domain;
using domain.Interfaces;
using domain.Models;
using domain.Services;
using domain.SqlEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace chldr_blazor_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.          
            ServiceRegistrator.RegisterCommonServices(builder.Services);

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });

            builder.Services.AddLocalization();
            builder.Services.AddBlazoredModal();

            builder.Services.AddScoped<IExceptionHandler, ExceptionHandler>();
            builder.Services.AddScoped<IEnvironmentService>(x => new EnvironmentService(Platforms.Web, builder.Environment.IsDevelopment()));
            builder.Services.AddScoped<ISettingsService, JsonFileSettingsService>();

            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<CultureService>();
            builder.Services.AddScoped<JsInteropService, JsInteropService>();

            // SQL Services **************************************************************
            var connectionString = builder.Configuration.GetConnectionString("SqlContext");
            builder.Services.AddDbContext<SqlContext>(options => options
                       .UseMySQL(connectionString, b => b.MigrationsAssembly("api_server")), ServiceLifetime.Transient);

            builder.Services.AddTransient<IDataProvider, SqlDataProvider>();
            builder.Services
                .AddDefaultIdentity<SqlUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<SqlContext>();
            builder.Services.AddHttpContextAccessor();

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            else if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();

            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}