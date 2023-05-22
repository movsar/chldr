using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils;
using GraphQL;

namespace chldr_data.Services
{
    public class AuthService
    {
        private readonly IGraphQLRequestSender _requestSender;        

        public AuthService(IGraphQLRequestSender requestSender)
        {
            _requestSender = requestSender;
        }

        public async Task PasswordResetRequestAsync(string email)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                mutation($email: String!) {
                    passwordReset(email: $email) {
                        success
                        errorMessage
                        resetToken
                    }
                }",
                Variables = new { email }
            };

            var response = await _requestSender.SendRequestAsync<PasswordResetResponse>(request, "passwordReset");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                mutation($token: String!, $newPassword: String!) {
                    updatePassword(token: $token, newPassword: $newPassword) {
                        success
                        errorMessage
                    }
                }",
                Variables = new { token, newPassword }
            };

            var response = await _requestSender.SendRequestAsync<MutationResponse>(request, "updatePassword");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }
        }

        public async Task ConfirmUserAsync(string token)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($token: String!) {
                            confirmEmail(token: $token) {
                                success
                                errorMessage
                            }
                        }",

                Variables = new { token }
            };

            var response = await _requestSender.SendRequestAsync<MutationResponse>(request, "confirmEmail");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }
        }

        internal async Task<ActiveSession> LogInEmailPasswordAsync(string email, string password)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($email: String!, $password: String!) {
                            loginEmailPassword(email: $email, password: $password) {
                                success
                                errorMessage
                                accessToken
                                refreshToken
                                accessTokenExpiresIn
                                user {
                                    userId,
                                    email,
                                    firstName,
                                    lastName,
                                    rate
                                }
                            }
                        }",

                Variables = new { email, password }
            };

            var response = await _requestSender.SendRequestAsync<LoginResponse>(request, "loginEmailPassword");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            return new ActiveSession()
            {
                AccessToken = response.Data.AccessToken,
                RefreshToken = response.Data.RefreshToken,
                AccessTokenExpiresIn = (DateTimeOffset)response.Data.AccessTokenExpiresIn,
                Status = SessionStatus.LoggedIn,
                User = response.Data.User
            };
        }

        internal async Task<string> RegisterUserAsync(string email, string password)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($email: String!, $password: String!) {
                            registerUser(email: $email, password: $password) {
                                success
                                errorMessage
                                token
                            }
                        }",

                Variables = new { email, password }
            };

            var response = await _requestSender.SendRequestAsync<RegistrationResponse>(request, "registerUser");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            if (string.IsNullOrWhiteSpace(response.Data.Token))
            {
                throw new Exception("Token shouldn't be null");
            }

            return response.Data.Token;
        }

        internal async Task<ActiveSession> RefreshTokens(string refreshToken)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($refreshToken: String!) {
                            logInRefreshToken(refreshToken: $refreshToken) {
                                success
                                errorMessage
                                accessToken
                                refreshToken
                                accessTokenExpiresIn
                                user {
                                    userId,
                                    email,
                                    firstName,
                                    lastName,
                                    rate
                                }
                            }
                        }",

                Variables = new { refreshToken }
            };

            var response = await _requestSender.SendRequestAsync<LoginResponse>(request, "logInRefreshToken");
            return new ActiveSession()
            {
                AccessToken = response.Data.Success ? response.Data.AccessToken! : "",
                RefreshToken = response.Data.Success ? response.Data.RefreshToken! : "",
                AccessTokenExpiresIn = response.Data.Success ? (DateTimeOffset)response.Data.AccessTokenExpiresIn! : DateTime.UtcNow,
                Status = response.Data.Success ? SessionStatus.LoggedIn : SessionStatus.Anonymous,
                User = response.Data.Success ? response.Data.User : null
            };
        }
    }
}
