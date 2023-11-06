using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Services;
using Microsoft.AspNetCore.Identity;
using chldr_data.sql.SqlEntities;
using chldr_data.Resources.Localizations;
using Microsoft.Extensions.Localization;

namespace chldr_data.sql.Services
{
    public class SqlDataProvider : IDataProvider
    {
        public bool IsInitialized { get; set; }
        public event Action? DatabaseInitialized;

        private readonly SqlContext _context;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;

        public SqlDataProvider(
            SqlContext context,
            FileService fileService,
            ExceptionHandler exceptionHandler)
        {
            _context = context;
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
        }

        public IDataAccessor CreateUnitOfWork(string? userId = null)
        {
            return new SqlDataAccessor(_context, _fileService, _exceptionHandler, userId);
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