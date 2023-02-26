using chldr_data.Resources.Localizations;
using chldr_shared.Models;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
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
        private readonly Serilog.Core.Logger _logger;

        #region Reset password
        // Fired when user submits password reset form
        [HttpGet("sendResetPasswordEmail")]
        public ActionResult SendResetPasswordEmail(string token, string tokenId, string email)
        {
            var resetPasswordLink = new Uri(QueryHelpers.AddQueryString($"{AppConstants.Host}/set-new-password", new Dictionary<string, string?>(){
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
            var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{AppConstants.Host}/login", new Dictionary<string, string?>(){
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
        public UserController(EmailService emailService, IStringLocalizer<AppLocalizations> localizer)
        {
            _localizer = localizer;
            _emailService = emailService;
            _logger = new LoggerConfiguration()
                          .WriteTo.File(Path.Combine(FileService.AppDataDirectory!, "logs", "log.txt"), rollingInterval: RollingInterval.Year)
                          .CreateLogger();
        }
        #endregion

    }
}
