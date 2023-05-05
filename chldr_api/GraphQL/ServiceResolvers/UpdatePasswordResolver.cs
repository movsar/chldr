using chldr_data.Enums;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace chldr_api.GraphQL.MutationServices
{
    public class UpdatePasswordResolver : ServiceResolver
    {
        public UpdatePasswordResolver(IConfiguration configuration, IStringLocalizer<AppLocalizations> localizer, EmailService emailService) : base(configuration, localizer, emailService)
        {}

        internal async Task<MutationResponse> ExecuteAsync(string token, string newPassword)
        {
            var tokenInDatabase = await dbContext.Tokens.FirstOrDefaultAsync(t => t.Type == (int)TokenType.PasswordReset && t.Value == token && t.ExpiresIn > DateTimeOffset.UtcNow);

            if (tokenInDatabase == null)
            {
                return new MutationResponse("Invalid token");
            }

            var user = await dbContext.Users.FindAsync(tokenInDatabase.UserId);
            if (user == null)
            {
                return new MutationResponse("User not found");
            }

            // Hash the new password and update the user's password in the Users table
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await dbContext.SaveChangesAsync();

            // Remove the used password reset token from the Tokens table
            dbContext.Tokens.Remove(tokenInDatabase);
            await dbContext.SaveChangesAsync();

            return new MutationResponse() { Success = true };
        }
    }
}
