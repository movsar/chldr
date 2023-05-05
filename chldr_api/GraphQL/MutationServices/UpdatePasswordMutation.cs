using chldr_data.Enums;
using chldr_data.ResponseTypes;
using chldr_tools;
using Microsoft.EntityFrameworkCore;

namespace chldr_api.GraphQL.MutationServices
{
    public class UpdatePasswordMutation : MutationService
    {
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
