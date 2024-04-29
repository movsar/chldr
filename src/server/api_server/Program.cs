using Microsoft.AspNetCore.HttpOverrides;

namespace api_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.ConfigureServices(builder.Configuration);

            builder.Services.AddLocalization();
        
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseHsts();
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            
            app.Map("/graphql", context =>
            {
                // Redirect all requests to example.com/graphql
                context.Response.Redirect("https://v1.api.chldr.movsar.dev/graphql", permanent: true, true);
                return Task.CompletedTask;
            });

            app.MapGraphQL("/v2/graphql");
            app.Run("https://*:5002");
        }
    }
}