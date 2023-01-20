using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_data.Services.PartialMethods;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Realms.Sync;
using Serilog;

namespace chldr_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly Serilog.Core.Logger _logger;

        public UserController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _logger = new LoggerConfiguration()
                          .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Year)
                          .CreateLogger();
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<ActionResult<string>> ConfirmEmail([FromQuery] string token, [FromQuery] string tokenId)
        {
            try
            {
                await _dataAccess.App.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
                return Redirect("https://nohchiyn-mott.com/login/?emailConfirmed=true");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Write(Serilog.Events.LogEventLevel.Debug, ex.StackTrace);

                return new { error = "Something went wrong :( " }.ToJson();
            }
        }

        [HttpGet("PasswordReset")]
        public async Task<ActionResult<string>> PasswordReset([FromQuery] string newPassword, [FromQuery] string token, [FromQuery] string tokenId)
        {
            try
            {
                await _dataAccess.App.EmailPasswordAuth.ResetPasswordAsync(newPassword, token, tokenId);
                return Redirect("https://nohchiyn-mott.com/login/?passwordChanged=true");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Write(Serilog.Events.LogEventLevel.Debug, ex.StackTrace);

                return new { error = "Something went wrong :( " }.ToJson();
            }
        }

    }
}
