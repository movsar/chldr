using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_data.Services.PartialMethods;
using chldr_shared;
using chldr_shared.Resources.Localizations;
using chldr_shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using Realms.Sync;
using Serilog;
using EmailService = chldr_shared.Services.EmailService;

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
        // Fired when user follows with the password reset link from the email,
        [HttpGet("resetPassword")]
        public async Task<ActionResult<bool>> ResetPassword(string token, string tokenId, string newPassword)
        {
            try
            {
                await _dataAccess.App.EmailPasswordAuth.ResetPasswordAsync(newPassword, token, tokenId);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        // Fired when user submits password reset form
        [HttpGet("sendResetPasswordEmail")]
        public ActionResult<bool> SendResetPasswordEmail(string token, string tokenId, string email)
        {
            var resetPasswordLink = new Uri(QueryHelpers.AddQueryString($"{Constants.Host}/set-new-password", new Dictionary<string, string?>(){
                { "token", token},
                { "tokenId", tokenId},
                { "email", email},
            })).ToString();

            var message = new EmailMessage(new string[] { "movsar.dev@gmail.com" },
              _localizer["Email:Reset_password_subject"],
              _localizer["Email:Reset_password_html", resetPasswordLink]);

            try
            {
                _emailService.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }
        #endregion

        #region Confirm email
        // Fired when user follows with the confirm email link from the email,
        [HttpGet("confirmEmail")]
        public async Task<ActionResult<bool>> Confirm(string token, string tokenId, string email)
        {
            try
            {
                await _dataAccess.App.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        // Fired when user submits registration form
        [HttpGet("sendConfirmationEmail")]
        public ActionResult<bool> SendConfirmationEmail(string token, string tokenId, string email)
        {
            var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{Constants.Host}/api/user/confirmEmail", new Dictionary<string, string?>(){
                { "token", token},
                { "tokenId", tokenId},
                { "email", email},
            })).ToString();

            var message = new EmailMessage(new string[] { "movsar.dev@gmail.com" },
                _localizer["Email:Confirm_email_subject"],
                _localizer["Email:Confirm_email_html", confirmEmailLink]);

            try
            {
                _emailService.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
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
                          .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Year)
                          .CreateLogger();
        }
        #endregion

    }
}
