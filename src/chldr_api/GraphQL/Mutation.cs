using chldr_api.GraphQL.MutationResolvers;
using chldr_api.GraphQL.MutationServices;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_tools;
using Microsoft.AspNetCore.Http.HttpResults;

namespace chldr_api
{
    public class Mutation
    {
        private readonly UserResolver _userResolver;
        private readonly EntryResolver _entryResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public Mutation(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            UserResolver userResolver,
            EntryResolver entryResolver)
        {
            _userResolver = userResolver;
            _entryResolver = entryResolver;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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

        // Entry mutations
        public async Task<RequestResult> PromoteAsync(string recordTypeName, string userId, string entryId)
        {
            var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);
            switch (recordType)
            {
                case RecordType.Entry:
                    return await Execute(() => _entryResolver.PromoteAsync(userId, entryId));

                case RecordType.Translation:
                    throw new NotImplementedException();

                default:
                    return new RequestResult();
            }
        }

        public async Task<RequestResult> AddEntry(string userId, EntryDto entryDto)
        {
         
                var accessToken = GetBearerToken();
                var signingKeyAsText = _configuration.GetValue<string>("ApiJwtSigningKey")!;
                var principal = JwtService.GetPrincipalFromAccessToken(accessToken, signingKeyAsText);

                return await Execute(() => _entryResolver.AddEntryAsync(userId, entryDto));
           
        }

        public async Task<RequestResult> UpdateEntry(string userId, EntryDto entryDto)
        {
            return await Execute(() => _entryResolver.UpdateEntry(userId, entryDto));
        }

        public async Task<RequestResult> RemoveEntry(string userId, string entryId)
        {
            return await Execute(() => _entryResolver.RemoveEntry(userId, entryId));
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
    }

}