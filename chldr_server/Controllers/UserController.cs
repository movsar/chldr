using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_ui;
using chldr_shared.Resources.Localizations;
using chldr_ui.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using Realms.Sync;
using Serilog;
using EmailService = chldr_ui.Services.EmailService;
using chldr_shared;
using chldr_shared.Models;

namespace chldr_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly EmailService _emailService;
        private readonly IDataAccess _dataAccess;
        private readonly Serilog.Core.Logger _logger;

        #region Reset password
        // Fired when user submits password reset form
        [HttpGet("sendResetPasswordEmail")]
        public ActionResult SendResetPasswordEmail(string token, string tokenId, string email)
        {
            var resetPasswordLink = new Uri(QueryHelpers.AddQueryString($"{Constants.Host}/set-new-password", new Dictionary<string, string?>(){
                { "token", token},
                { "tokenId", tokenId},
            })).ToString();

            var message = new EmailMessage(new string[] { email },
              _localizer["Email:Reset_password_subject"],
              _localizer["Email:Reset_password_html", resetPasswordLink]);

            try
            {
                _emailService.Send(message);
                return Ok();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new BadRequestResult();
            }
        }
        #endregion

        #region Confirm email
        // Fired when user submits registration form
        [HttpGet("sendConfirmationEmail")]
        public ActionResult SendConfirmationEmail(string token, string tokenId, string email)
        {
            var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{Constants.Host}/login", new Dictionary<string, string?>(){
                { "token", token},
                { "tokenId", tokenId},
                { "email", email},
            })).ToString();

            var message = new EmailMessage(new string[] { email },
                _localizer["Email:Confirm_email_subject"],
                _localizer["Email:Confirm_email_html", confirmEmailLink]);

            try
            {
                _emailService.Send(message);
                return Ok();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new BadRequestResult();
            }
        }
        #endregion

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Where's the will, there's the way";
        }

        private void LogError(Exception ex)
        {
            _logger.Error(ex.Message);
            _logger.Write(Serilog.Events.LogEventLevel.Debug, ex.StackTrace);
        }

        #region Constructors
        public UserController(IDataAccess dataAccess, EmailService emailService, IStringLocalizer<AppLocalizations> localizer)
        {
            _localizer = localizer;
            _emailService = emailService;
            _dataAccess = dataAccess;
            _logger = new LoggerConfiguration()
                          .WriteTo.File(Path.Combine(FileService.AppDataDirectory!, "logs", "log.txt"), rollingInterval: RollingInterval.Year)
                          .CreateLogger();
        }
        #endregion

    }
}
