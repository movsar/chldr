using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using GraphQL;
using Newtonsoft.Json;

namespace chldr_data.Services
{
    public class RequestService
    {
        public RequestService(IGraphQlClient graphQLRequestSender)
        {
            _graphQLRequestSender = graphQLRequestSender;
        }
        private IGraphQlClient _graphQLRequestSender;
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

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "updatePassword");
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

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "confirmEmail");
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

            var response = await _graphQLRequestSender.SendRequestAsync<LoginResult>(request, "loginEmailPassword");
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

            var response = await _graphQLRequestSender.SendRequestAsync<RegistrationResult>(request, "registerUser");
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

            var response = await _graphQLRequestSender.SendRequestAsync<LoginResult>(request, "logInRefreshToken");
            return new ActiveSession()
            {
                AccessToken = response.Data.Success ? response.Data.AccessToken! : "",
                RefreshToken = response.Data.Success ? response.Data.RefreshToken! : "",
                AccessTokenExpiresIn = response.Data.Success ? (DateTimeOffset)response.Data.AccessTokenExpiresIn! : DateTime.UtcNow,
                Status = response.Data.Success ? SessionStatus.LoggedIn : SessionStatus.Anonymous,
                User = response.Data.Success ? response.Data.User : null
            };
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

            var response = await _graphQLRequestSender.SendRequestAsync<PasswordResetResult>(request, "passwordReset");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }
        }
        public async Task<IEnumerable<T>> TakeLast<T>(RecordType recordType, int count)
        {
            var operation = "takeLast";
            var request = new GraphQLRequest
            {
                Query = $@"
                        query {operation}($recordTypeName: String!, $count: Int!) {{
                          {operation}(recordTypeName: $recordTypeName, count: $count) {{
                            success
                            errorMessage
                            serializedData
                          }}
                        }}",

                Variables = new { recordTypeName = recordType.ToString(), count }
            };
            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            var listOfObjects = JsonConvert.DeserializeObject<IEnumerable<T>>(response.Data.SerializedData);
            return listOfObjects;
        }

        public async Task<IEnumerable<T>> Take<T>(RecordType recordType, int offset, int limit)
        {
            var operation = "take";
            var request = new GraphQLRequest
            {
                Query = $@"
                        query {operation}($recordTypeName: String!, $offset: Int!, $limit: Int!) {{
                          {operation}(recordTypeName: $recordTypeName, offset: $offset, limit: $limit) {{
                            success
                            errorMessage
                            serializedData
                          }}
                        }}",

                Variables = new { recordTypeName = recordType.ToString(), offset, limit }
            };
            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            var listOfObjects = JsonConvert.DeserializeObject<IEnumerable<T>>(response.Data.SerializedData);
            return listOfObjects;
        }

        public async Task<InsertResult> AddEntry(string userId, EntryDto entryDto)
        {
            var operation = "addEntry";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}($userId: String!, $entryDto: EntryDtoInput!) {{
                          {operation}(userId: $userId, entryDto: $entryDto) {{
                            success
                            errorMessage
                            createdAt
                          }}
                        }}
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { userId, entryDto }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<InsertResult>(request, operation);
            return response.Data;
        }

        public async Task<RequestResult> RemoveEntry(string userId, string entityId)
        {
            var operation = "removeEntry";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}($userId: String!, $entryId: String!) {{
                          {operation}(userId: $userId, entryId: $entryId) {{
                            success
                            errorMessage
                          }}
                        }}
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { userId, entryId = entityId }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            return response.Data;
        }

        public async Task<RequestResult> UpdateEntry(string userId, EntryDto entryDto)
        {
            var operation = "updateEntry";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}($userId: String!, $entryDto: EntryDtoInput!) {{
                          {operation}(userId: $userId, entryDto: $entryDto) {{
                            success
                            errorMessage
                          }}
                        }}
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { userId, entryDto }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            return response.Data;
        }
    }
}
