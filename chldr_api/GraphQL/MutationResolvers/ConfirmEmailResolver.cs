using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.Services;
using chldr_data.Models;
using chldr_data.remote.Repositories;
using chldr_data.remote.Services;
using chldr_data.Services;
using chldr_tools;
using Microsoft.EntityFrameworkCore;

namespace chldr_api.GraphQL.MutationServices
{
    public class ConfirmEmailResolver
    {
        internal async Task<RequestResult> ExecuteAsync(SqlDataProvider dataProvider, string tokenValue)
        {
            using var unitOfWork = (SqlUnitOfWork)dataProvider.CreateUnitOfWork();

            // Check if a user with this email already exists
            var usersRepository = (SqlUsersRepository)unitOfWork.Users;
            var tokensRepository = (SqlTokensRepository)unitOfWork.Tokens;

            var token = await tokensRepository.FindByValueAsync(tokenValue);
            if (token == null)
            {
                return new RequestResult() { ErrorMessage = "Invalid token" };
            }

            var isExpired = JwtService.IsTokenExpired(token.Value);
            if (isExpired)
            {
                return new RequestResult() { ErrorMessage = "Token has expired " };
            }

            var user = usersRepository.SetStatus(token.UserId, UserStatus.Active);

            return new RequestResult();
        }
    }
}
