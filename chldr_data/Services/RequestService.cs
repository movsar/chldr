using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using GraphQL;

namespace chldr_data.Services
{
    public class RequestService
    {
        public RequestService(IGraphQlClient graphQLRequestSender)
        {
            _graphQLRequestSender = graphQLRequestSender;
        }
        private IGraphQlClient _graphQLRequestSender;
        public async Task<RequestResult> UpdatePasswordAsync(string token, string newPassword)
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
            return response.Data;
        }

        public async Task<RequestResult> ConfirmUserAsync(string token)
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
            return response.Data;
        }

        internal async Task<RequestResult> LogInEmailPasswordAsync(string email, string password)
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
            return response.Data;
        }

        internal async Task<RequestResult> RegisterUserAsync(string email, string password)
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
            return response.Data;
        }

        internal async Task<RequestResult> RefreshTokens(string refreshToken)
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
            return response.Data;
        }
        public async Task<RequestResult> PasswordResetRequestAsync(string email)
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
            return response.Data;
        }
        public async Task<RequestResult> TakeLast<T>(RecordType recordType, int count)
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
            return response.Data;
        }

        public async Task<RequestResult> Take<T>(RecordType recordType, int offset, int limit)
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
            return response.Data;
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
