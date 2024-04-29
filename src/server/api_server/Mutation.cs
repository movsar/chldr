using chldr_api.GraphQL.MutationResolvers;
using chldr_api.GraphQL.MutationServices;
using core.DatabaseObjects.Dtos;
using core.Enums;
using core.Models;
using chldr_tools;

namespace api_server
{
    public class Mutation
    {
        private readonly UserResolver _userResolver;
        private readonly EntryResolver _entryResolver;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Mutation(
            IConfiguration configuration,
            UserResolver userResolver,
            EntryResolver entryResolver,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userResolver = userResolver;
            _entryResolver = entryResolver;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUserId()
        {
            var accessToken = GetBearerToken();
            var signingKeyAsText = _configuration.GetValue<string>("ApiJwtSigningKey")!;
            var principal = JwtService.GetPrincipalFromAccessToken(accessToken, signingKeyAsText);
            return principal.Identity?.Name!;
        }

        // Entry mutations
        public async Task<RequestResult> AddSoundAsync(PronunciationDto pronunciation)
        {
            var currentUserId = GetCurrentUserId();
            return await Execute(() => _entryResolver.AddSoundAsync(currentUserId, pronunciation));
        }

        public async Task<RequestResult> PromoteAsync(string recordTypeName, string entryId)
        {
            var currentUserId = GetCurrentUserId();

            var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);
            switch (recordType)
            {
                case RecordType.Entry:
                    return await Execute(() => _entryResolver.PromoteAsync(currentUserId, entryId));

                case RecordType.Translation:
                    throw new NotImplementedException();

                default:
                    return new RequestResult();
            }
        }

        public async Task<RequestResult> AddEntry(EntryDto entryDto)
        {
            var currentUserId = GetCurrentUserId();
            return await Execute(() => _entryResolver.AddEntryAsync(currentUserId, entryDto));
        }

        public async Task<RequestResult> UpdateEntry(EntryDto entryDto)
        {
            var currentUserId = GetCurrentUserId();
            return await Execute(() => _entryResolver.UpdateEntry(currentUserId, entryDto));
        }

        public async Task<RequestResult> RemoveEntry(string entryId)
        {
            var currentUserId = GetCurrentUserId();
            return await Execute(() => _entryResolver.RemoveEntry(currentUserId, entryId));
        }

        // User mutations
        public async Task<RequestResult> UpdatePasswordAsync(string email, string token, string newPassword)
        {
            return await Execute(() => _userResolver.SetNewPassword(email, token, newPassword));
        }
        public async Task<RequestResult> PasswordReset(string email)
        {
            return await Execute(() => _userResolver.ResetPassword(email));
        }
        public async Task<RequestResult> RefreshTokens(string accessToken, string refreshToken)
        {
            return await Execute(() => _userResolver.RefreshTokens(accessToken, refreshToken));
        }
        public async Task<RequestResult> LoginEmailPasswordAsync(string email, string password)
        {
            return await Execute(() => _userResolver.SignInAsync(email, password));
        }
        public async Task<RequestResult> ConfirmEmailAsync(string token)
        {
            return await Execute(() => _userResolver.ConfirmEmailAsync(token));
        }
        public async Task<RequestResult> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            return await Execute(() => _userResolver.RegisterAndLogInAsync(email, password, firstName, lastName, patronymic));
        }

        private async Task<RequestResult> Execute(Func<Task<RequestResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                return new RequestResult()
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public string GetBearerToken()
        {
            string authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                // Handle missing Bearer token appropriately
                throw new Exception("Missing or invalid Authorization header");
            }
            return authHeader.Split(' ')[1];
        }
    }

}