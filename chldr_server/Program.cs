using Blazored.Modal;
using chldr_blazor.Extensions;
using chldr_shared.Enums;
using chldr_utils.Services;
using System.Reflection.PortableExecutable;

namespace chldr_blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });

            builder.Services.AddLocalization();
            builder.Services.AddBlazoredModal();

            builder.RegisterWebAppServices();
           
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