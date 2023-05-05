using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.ResponseTypes;
using chldr_tools;
using Microsoft.EntityFrameworkCore;

namespace chldr_api.GraphQL.MutationServices
{
    public class RegisterUserMutation : MutationService
    {
        internal async Task<MutationResponse> ExecuteAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            // Check if a user with this email already exists
            var existingUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                return new MutationResponse("A user with this email already exists");
            }

            // Create the new user
            var user = new SqlUser
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                AccountStatus = (int)UserStatus.Unconfirmed,
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var confirmationTokenExpiration = DateTime.UtcNow.AddDays(30);
            var confirmationToken = JwtService.GenerateToken(user.UserId, "confirmation-token-secret", confirmationTokenExpiration);

            // Save the tokens to the database
            dbContext.Tokens.Add(new SqlToken
            {
                UserId = user.UserId,
                Type = (int)TokenType.Confirmation,
                Value = confirmationToken,
                ExpiresIn = confirmationTokenExpiration
            });

            // TODO: Send email with confirmation link

            return new MutationResponse() { Success = true };
        }
    }
}
