using Data.Services;
using Data.Services.PartialMethods;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Realms.Sync;
using Serilog;

namespace user_management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataAccess _dataAccess;
        private readonly Serilog.Core.Logger _logger;

        public UserController(DataAccess dataAccess)
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
                var app = App.Create(Data.Constants.myRealmAppId);
                await app.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
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
        public IEnumerable<string> PasswordReset([FromQuery] string token, [FromQuery] string tokenId)
        {
            return new string[] { token, tokenId };
        }

    }
}
