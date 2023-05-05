using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace chldr_api.GraphQL.MutationServices
{
    public class ConfirmEmailResolver : ServiceResolver
    {
        public ConfirmEmailResolver(IConfiguration configuration, IStringLocalizer<AppLocalizations> localizer, EmailService emailService) : base(configuration, localizer, emailService)
        {
        }

        internal async Task<MutationResponse> ExecuteAsync(string tokenValue)
        {
            // Check if a user with this email already exists
            var token = await dbContext.Tokens.SingleOrDefaultAsync(t => t.Value.Equals(tokenValue));
            if (token == null)
            {
                return new MutationResponse() { ErrorMessage = "Invalid token" };
            }

            var isExpired = JwtService.IsTokenExpired(token.Value);
            if (isExpired)
            {
                return new MutationResponse() { ErrorMessage = "Token has expired " };
            }

            var user = dbContext.Users.First(u => u.UserId.Equals(token.UserId));
            user.UserStatus = (int)UserStatus.Active;
            await dbContext.SaveChangesAsync();

            return new MutationResponse();
        }
    }
}
