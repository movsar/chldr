using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.local.Services;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.Services;
using Microsoft.EntityFrameworkCore;

namespace chldr_api.GraphQL.MutationServices
{
    public class UpdatePasswordResolver
    {
        internal async Task<RequestResult> ExecuteAsync(SqlDataProvider dataProvider, string tokenValue, string newPassword)
        {
            var unitOfWork = (SqlUnitOfWork)dataProvider.CreateUnitOfWork();
            unitOfWork.BeginTransaction();

            var tokenInDatabase = await unitOfWork.Tokens.GetPasswordResetTokenAsync(tokenValue);
            if (tokenInDatabase == null)
            {
                return new RequestResult("Invalid token");
            }

            var user = unitOfWork.Users.Get(tokenInDatabase.UserId);
            if (user == null)
            {
                return new RequestResult("User not found");
            }

            var userDto = UserDto.FromModel(user);

            // Hash the new password and update the user's password in the Users table
            userDto.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            unitOfWork.Users.Update(userDto);

            // Remove the used password reset token from the Tokens table
            unitOfWork.Tokens.Remove(tokenInDatabase.TokenId);

            unitOfWork.Commit();

            return new RequestResult() { Success = true };
        }
    }
}
