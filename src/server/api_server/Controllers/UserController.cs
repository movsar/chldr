using api_server.GraphQL.MutationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_server.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserResolver _userResolver;
        private readonly IHttpContextAccessor _httpContext;
        public UserController(UserResolver userResolver, IHttpContextAccessor httpContext)
        {
            _userResolver = userResolver;
            _httpContext = httpContext;
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            try
            {
                await _userResolver.ConfirmEmailAsync(token);
                return Ok("Ваша учетная запись успешно активирована! Это можно закрыть.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
