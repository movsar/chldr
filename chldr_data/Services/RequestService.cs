using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using GraphQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms.Sync;

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

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "loginEmailPassword");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            var data = JsonConvert.DeserializeObject<dynamic>(response.Data.SerializedData);
            if (data == null)
            {
                throw new Exception("Error:Data_cannot_be_null");
            }

            return new ActiveSession()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                AccessTokenExpiresIn = data.AccessTokenExpiresIn!,
                Status = SessionStatus.LoggedIn,
                User = data.User
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

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "registerUser");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            var token = JsonConvert.DeserializeObject<string>(response.Data.SerializedData);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("Token shouldn't be null");
            }

            return token;
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

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "logInRefreshToken");

            if (!response.Data.Success)
            {
                throw new Exception("Error:Bad_request");
            }

            var data = JsonConvert.DeserializeObject<dynamic>(response.Data.SerializedData);
            if (data == null)
            {
                throw new Exception("Error:Data_cannot_be_null");
            }

            return new ActiveSession()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                AccessTokenExpiresIn = data.AccessTokenExpiresIn!,
                Status = SessionStatus.LoggedIn,
                User = data.User
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

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "passwordReset");
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

        public async Task<RequestResult> AddEntry(string userId, EntryDto entryDto)
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

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
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
