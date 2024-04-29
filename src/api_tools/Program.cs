using api_domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using api_server;
using api_domain;

namespace api_tools
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var configuration = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .Build();

            IServiceCollection services = new ServiceCollection();
            services.ConfigureServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var _dataProvider = serviceProvider.GetRequiredService<SqlDataProvider>();
            using var unitOfWork = (SqlDataAccessor)_dataProvider.Repositories();

            var dbContext = serviceProvider.GetRequiredService<SqlContext>();

            object? models = null;

            foreach (var user in dbContext.Users)
            {

            }

        }
    }
}
