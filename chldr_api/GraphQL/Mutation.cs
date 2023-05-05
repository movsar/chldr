using chldr_api.GraphQL.MutationServices;
using chldr_data.Resources.Localizations;
using chldr_data.ResponseTypes;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.Extensions.Localization;

namespace chldr_api
{
    public class Mutation
    {
        private readonly PasswordResetMutation _passwordResetMutation;
        private readonly UpdatePasswordMutation _updatePasswordMutation;
        private readonly RegisterUserMutation _registerUserMutation;
        private readonly LoginUserMutation _loginUserMutation;

        public Mutation(
            PasswordResetMutation passwordResetMutation,
            UpdatePasswordMutation updatePasswordMutation,
            RegisterUserMutation registerUserMutation,
            LoginUserMutation loginUserMutation)
        {
            _passwordResetMutation = passwordResetMutation;
            _updatePasswordMutation = updatePasswordMutation;
            _registerUserMutation = registerUserMutation;
            _loginUserMutation = loginUserMutation;
        }

        public async Task<PasswordResetResponse> PasswordReset(string email)
        {
            return await _passwordResetMutation.ExecuteAsync(email);
        }

        public async Task<MutationResponse> UpdatePasswordAsync(string token, string newPassword)
        {
            return await _updatePasswordMutation.ExecuteAsync(token, newPassword);
        }

        public async Task<RegistrationResponse> RegisterUserAsync(string email, string password, string? firstName, string? lastName, string? patronymic)
        {
            return await _registerUserMutation.ExecuteAsync(email, password, firstName, lastName, patronymic);
        }

        public async Task<LoginResponse> LoginUserAsync(string email, string password)
        {
            return await _loginUserMutation.ExecuteAsync(email, password);
        }
    }

}