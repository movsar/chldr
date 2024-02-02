using api_domain.Interfaces;
using chldr_data.Interfaces;
using chldr_data.Services;

namespace api_domain.Services
{
    public class SqlDataProvider : IDataProvider
    {
        public bool IsInitialized { get; set; }
        public event Action? DatabaseInitialized;

        private readonly SqlContext _context;
        private readonly IFileService _fileService;
        private readonly IExceptionHandler _exceptionHandler;

        public SqlDataProvider(
            SqlContext context,
            IFileService fileService,
            IExceptionHandler exceptionHandler)
        {
            _context = context;
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
        }

        public IDataAccessor Repositories(string? userId = null)
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