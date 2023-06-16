using chldr_data.Enums;
using chldr_data.local.Services;
using chldr_data.remote.Services;
using chldr_data.ResponseTypes;
using chldr_tools;
using Microsoft.EntityFrameworkCore;

namespace chldr_api.GraphQL.MutationServices
{
    public class ConfirmEmailResolver
    {
        internal async Task<OperationResult> ExecuteAsync(SqlDataProvider dataProvider, string tokenValue)
        {
            var dbContext = dataProvider.GetContext();

            // Check if a user with this email already exists
            var token = await dbContext.Tokens.SingleOrDefaultAsync(t => t.Value.Equals(tokenValue));
            if (token == null)
            {
                return new OperationResult() { ErrorMessage = "Invalid token" };
            }

            var isExpired = JwtService.IsTokenExpired(token.Value);
            if (isExpired)
            {
                return new OperationResult() { ErrorMessage = "Token has expired " };
            }

            var user = dbContext.Users.First(u => u.UserId.Equals(token.UserId));
            user.Status = (int)UserStatus.Active;
            await dbContext.SaveChangesAsync();

            return new OperationResult();
        }
    }
}
