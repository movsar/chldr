using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Services;
using chldr_utils.Interfaces;
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
        private static DbContextOptions<SqlContext> _options;

        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, IConfiguration configuration)
        {
            _fileService = fileService;
            //_connectionString = configuration.GetConnectionString("RemoteDatabase")!;
            _connectionString = Constants.TestDatabaseConnectionString;
            _exceptionHandler = exceptionHandler;
            _options ??= new DbContextOptionsBuilder<SqlContext>().UseMySQL(_connectionString).Options;
        }
        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, string connectionString)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _connectionString = connectionString;
            _options ??= new DbContextOptionsBuilder<SqlContext>().UseMySQL(_connectionString).Options;
        }
        public SqlContext GetContext()
        {
            return new SqlContext(_options);
        }

        public IUnitOfWork CreateUnitOfWork(string userId = Constants.DefaultUserId)
        {
            var context = GetContext();
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