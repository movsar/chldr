using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_data.Services.PartialMethods;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Realms.Sync;
using Serilog;

namespace chldr_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly Serilog.Core.Logger _logger;


        // Fired when user submits password reset form
        [HttpGet("sendResetPasswordEmail")]
        public ActionResult<string> SendResetPasswordEmail(string token, string tokenId, string email)
        {
            return $"token:{token}, tokenId:{tokenId}, email:{email}";
        }

        // Fired when user follows with the password reset link from the email,
        [HttpGet("resetPassword")]
        public async Task<ActionResult<bool>> ResetPassword([FromQuery] string newPassword, [FromQuery] string email, [FromQuery] string token, [FromQuery] string tokenId)
        {
            try
            {
                await _dataAccess.App.EmailPasswordAuth.ResetPasswordAsync(newPassword, token, tokenId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Write(Serilog.Events.LogEventLevel.Debug, ex.StackTrace);

                return false;
            }
        }

        // Fired when user submits registration form
        [HttpGet("sendConfirmationEmail")]
        public ActionResult<string> SendConfirmationEmail(string email, string token, string tokenId)
        {
            return $"token:{token}, tokenId:{tokenId}, email:{email}";
        }

        // Fired when user follows with the confirm email link from the email,
        [HttpGet("confirmEmail")]
        public async Task<ActionResult<bool>> Confirm([FromQuery] string token, [FromQuery] string tokenId)
        {
            try
            {
                await _dataAccess.App.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Write(Serilog.Events.LogEventLevel.Debug, ex.StackTrace);

                return false;
            }
        }

      

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Where's the will, there's the way";
        }

        public UserController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _logger = new LoggerConfiguration()
                          .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Year)
                          .CreateLogger();
        }

    }
}
