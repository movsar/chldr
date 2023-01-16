using Data.Services;
using Microsoft.AspNetCore.Mvc;
using Realms.Sync;

namespace user_management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataAccess _dataAccess;
        public UserController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IEnumerable<string>> ConfirmEmail([FromQuery] string token, [FromQuery] string tokenId)
        {
            var app = App.Create(Data.Constants.myRealmAppId);
            await app.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);

            return new string[] { "Your email has been successfully confirmed!" };
        }

        [HttpGet("PasswordReset")]
        public IEnumerable<string> PasswordReset([FromQuery] string token, [FromQuery] string tokenId)
        {
            return new string[] { token, tokenId };
        }

    }
}
