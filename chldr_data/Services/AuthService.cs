using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task ConfirmUserAsync(string token, string tokenId, string userEmail)
        {
            // TODO: change await App.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
        }

        internal async Task<ActiveSession> LogInEmailPasswordAsync(string email, string password)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($email: String!, $password: String!) {
                            loginUser(email: $email, password: $password) {
                                success
                                errorMessage
                                accessToken
                                refreshToken
                                accessTokenExpiresIn
                                user {
                                    email,
                                    firstName,
                                    lastName,
                                    rate
                                }
                            }
                        }",

                Variables = new { email, password }
            };

            var response = await _requestSender.SendRequestAsync<LoginResponse>(request, "loginUser");
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

            if (string.IsNullOrWhiteSpace(response.Data.Token)) {
                throw new Exception("Token shouldn't be null");
            }

            return response.Data.Token;
        }
    }
}
