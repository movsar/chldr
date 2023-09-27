using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Hosting;
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

        private DbContextOptions<SqlContext> _options;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly UserManager<SqlUser> _userManager;
        private readonly EmailService _emailService;
        private readonly SignInManager<SqlUser> _signInManager;

        public SqlDataProvider(
            FileService fileService,
            ExceptionHandler exceptionHandler,
            IConfiguration configuration,
            UserManager<SqlUser> userManager,
            SignInManager<SqlUser> signInManager,
            EmailService emailService,
            IStringLocalizer<AppLocalizations> localizer)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _localizer = localizer;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;

            var connectionString = configuration.GetConnectionString("SqlContext");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new NullReferenceException("Connection string is empty");
            }

            _options = new DbContextOptionsBuilder<SqlContext>()
               .UseMySQL(connectionString)
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

        public IUnitOfWork CreateUnitOfWork(string? userId = null)
        {
            return new SqlUnitOfWork(_options, _fileService, _exceptionHandler, _userManager, _signInManager, _emailService, _localizer, userId);
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