using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.local.Services;
using chldr_data.Models;
using chldr_data.remote.Repositories;
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
            var tokensRepository = (SqlTokensRepository)unitOfWork.Tokens;

            unitOfWork.BeginTransaction();

            var tokenInDatabase = await tokensRepository.GetPasswordResetTokenAsync(tokenValue);
            if (tokenInDatabase == null)
            {
                return new RequestResult("Invalid token");
            }

            var user = await unitOfWork.Users.Get(tokenInDatabase.UserId);
            if (user == null)
            {
                return new RequestResult("User not found");
            }

            var userDto = UserDto.FromModel(user);

            // Hash the new password and update the user's password in the Users table
            userDto.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await unitOfWork.Users.Update(userDto, null);

            // Remove the used password reset token from the Tokens table
            await unitOfWork.Tokens.Remove(tokenInDatabase.TokenId, null);

            unitOfWork.Commit();

            return new RequestResult() { Success = true };
        }
    }
}
