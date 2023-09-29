using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Services;
using Microsoft.AspNetCore.Identity;
using chldr_data.remote.SqlEntities;
using chldr_data.Resources.Localizations;
using Microsoft.Extensions.Localization;

namespace chldr_data.remote.Services
{
    public class SqlDataProvider : IDataProvider
    {
        public bool IsInitialized { get; set; }
        public event Action? DatabaseInitialized;

        private readonly SqlContext _context;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly EmailService _emailService;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly UserManager<SqlUser> _userManager;
        private readonly SignInManager<SqlUser> _signInManager;

        public SqlDataProvider(
            SqlContext context,
            FileService fileService,
            ExceptionHandler exceptionHandler,
            EmailService emailService,
            UserManager<SqlUser> userManager,
            SignInManager<SqlUser> signInManager,
            IStringLocalizer<AppLocalizations> localizer
            )
        {
            _context = context;
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _emailService = emailService;
            _localizer = localizer;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, string connectionString)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
        }

        public IUnitOfWork CreateUnitOfWork(string? userId = null)
        {
            return new SqlUnitOfWork(_context, _fileService, _exceptionHandler, _userManager, _signInManager, _emailService, _localizer, userId);
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