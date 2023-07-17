using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.remote.Services;
using chldr_data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace chldr_data.local.Services
{
    public class SqlDataProvider : IDataProvider
    {
        public bool IsInitialized { get; set; }

        public event Action? DatabaseInitialized;

        private readonly FileService _fileService;
        string _connectionString;
        private readonly ExceptionHandler _exceptionHandler;

        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, IConfiguration configuration)
        {
            _fileService = fileService;
            _connectionString = Constants.LocalDatabaseConnectionString;
            //_connectionString = configuration.GetConnectionString("SqlContext")!;
            _exceptionHandler = exceptionHandler;
        }
        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, string connectionString)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _connectionString = connectionString;
        }

        public IUnitOfWork CreateUnitOfWork(string userId = Constants.DefaultUserId)
        {
            var options = new DbContextOptionsBuilder<SqlContext>()
                .UseMySQL(_connectionString)
                .Options;

            var context = new SqlContext(options);
            return new SqlUnitOfWork(context, _fileService, _exceptionHandler, userId!);
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