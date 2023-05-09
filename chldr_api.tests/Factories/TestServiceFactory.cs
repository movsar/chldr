using chldr_data.Resources.Localizations;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace chldr_api.tests.Factories
{
    internal class TestServiceFactory
    {
        internal static IConfiguration CreateTestConfiguration()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        internal static EmailService CreateTestEmailService()
        {
            return new Mock<EmailService>().Object;
        }

        internal static IStringLocalizer<AppLocalizations> CreateTestStringLocalizer()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            return new StringLocalizer<AppLocalizations>(factory);
        }

        internal static SqlContext CreateTestDbContext()
        {
            return new SqlContext(CreateDbContextOptions());
        }
        
        private static DbContextOptions<SqlContext> CreateDbContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<SqlContext>();
            builder.UseInMemoryDatabase("TestDatabase")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
