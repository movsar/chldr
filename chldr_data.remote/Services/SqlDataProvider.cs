using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace chldr_data.remote.Services
{
    public class SqlDataProvider : IDataProvider
    {
        public bool IsInitialized { get; set; }
        private static string _actingUserId;
        public string ActingUserId => _actingUserId;

        public event Action? DatabaseInitialized;

        private readonly FileService _fileService;
        private DbContextOptions<SqlContext> _options;
        private readonly ExceptionHandler _exceptionHandler;

        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, IConfiguration configuration)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;

            _options = new DbContextOptionsBuilder<SqlContext>()
               .UseMySQL(configuration.GetConnectionString("SqlContext")!)
               .Options;
        }
        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, string connectionString)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;

            _options = new DbContextOptionsBuilder<SqlContext>()
               .UseMySQL(connectionString)
               .Options;
        }

        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, DbContextOptions<SqlContext> dbContextOptionsBuilder)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;

            _options = dbContextOptionsBuilder;
        }

        public IUnitOfWork CreateUnitOfWork(string? userId = null)
        {
            var context = new SqlContext(_options);
            if (!string.IsNullOrEmpty(userId))
            {
                _actingUserId = userId;
            }
            else if (string.IsNullOrEmpty(_actingUserId))
            {
                _actingUserId = context.Users.First().UserId;
            }

            return new SqlUnitOfWork(context, _fileService, _exceptionHandler, _actingUserId!);
        }

        public void Initialize()
        {
            DatabaseInitialized?.Invoke();
        }

        public void TruncateDatabase()
        {

        }

    }
}