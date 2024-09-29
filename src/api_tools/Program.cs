using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using api_server;
using core;

namespace api_tools
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .Build();

            IServiceCollection services = new ServiceCollection();
            services.ConfigureServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<SqlContext>();

            //foreach (var entry in dbContext.Entries.Include(e => e.Translations))
            //{
            //    foreach (var translation in entry.Translations)
            //    {
            //        translation.SourceId = entry.SourceId;
            //    }
            //}
            //await dbContext.SaveChangesAsync();
        }
    }
}
