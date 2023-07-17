using Blazored.Modal;
using chldr_data.Interfaces;
using chldr_data.local.Services;
using chldr_shared.Enums;
using chldr_utils.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace chldr_blazor
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

            builder.Services.AddScoped<IDataProvider, RealmDataProvider>();
            builder.Services.AddScoped<ISearchService, RealmSearchService>();
            builder.Services.AddScoped<SyncService>();

            builder.Services.AddSingleton(x => new EnvironmentService(Platforms.Web, builder.Environment.IsDevelopment()));


            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseStaticFiles();
            app.UseRouting();

            app.MapBlazorHub();

            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}