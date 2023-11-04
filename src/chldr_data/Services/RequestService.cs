using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_utils.Interfaces;
using GraphQL;

namespace chldr_data.Services
{
    public class RequestService
    {
        public async Task<RequestResult> AddSoundAsync(PronunciationDto pronunciation)
        {
            var operation = "addSound";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}($pronunciation: PronunciationDto!) {{
                          {operation}(pronunciation: $pronunciation) {{
                            success
                            errorMessage
                            serializedData
                          }}
                        }}
                        ",

                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { pronunciation }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            return response.Data;
        }

        public async Task<RequestResult> UpdatePasswordAsync(string email, string token, string newPassword)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                    mutation($email: String!, $token: String!, $newPassword: String!) {
                    updatePassword(email: $email, token: $token, newPassword: $newPassword) {
                        success
                        errorMessage
                        serializedData
                    }
                }",
                Variables = new { email, token, newPassword }
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
                                serializedData
                            }
                        }",

                Variables = new { token }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "confirmEmail");
            var data = response.Data;

            if (!data.Success)
            {
                throw new Exception(data.ErrorMessage);
            }
            return data;
        }

        public async Task<RequestResult> LogInEmailPasswordAsync(string email, string password)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($email: String!, $password: String!) {
                            loginEmailPassword(email: $email, password: $password) {
                                success
                                errorMessage
                                serializedData
                            }
                        }",

                Variables = new { email, password }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "loginEmailPassword");
            return response.Data;
        }

        public async Task<RequestResult> RegisterUserAsync(string email, string password)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($email: String!, $password: String!) {
                            registerUser(email: $email, password: $password) {
                                success
                                errorMessage
                                serializedData
                            }
                        }",

                Variables = new { email, password }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "registerUser");
            return response.Data;
        }

        public async Task<RequestResult> RefreshTokens(string accessToken, string refreshToken)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation($accessToken: String!, $refreshToken: String!) {
                            refreshTokens(accessToken: $accessToken,refreshToken: $refreshToken) {
                                success
                                errorMessage
                                serializedData
                            }
                        }",

                Variables = new { accessToken, refreshToken }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "refreshTokens");
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
                        serializedData
                    }
                }",
                Variables = new { email }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, "passwordReset");
            return response.Data;
        }
        public async Task<RequestResult> TakeLastAsync(RecordType recordType, int count)
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

        public async Task<RequestResult> TakeAsync(RecordType recordType, int offset, int limit)
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

        public async Task<RequestResult> AddEntryAsync(EntryDto entryDto)
        {
            var operation = "addEntry";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}($entryDto: EntryDtoInput!) {{
                          {operation}( entryDto: $entryDto) {{
                            success
                            errorMessage
                            serializedData
                          }}
                        }}
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new {  entryDto }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            return response.Data;
        }

        public async Task<RequestResult> RemoveEntry(string entityId)
        {
            var operation = "removeEntry";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}( $entryId: String!) {{
                          {operation}( entryId: $entryId) {{
                            success
                            errorMessage
                            serializedData
                          }}
                        }}
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { entryId = entityId }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            return response.Data;
        }

        public async Task<RequestResult> UpdateEntry( EntryDto entryDto)
        {
            var operation = "updateEntry";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}( $entryDto: EntryDtoInput!) {{
                          {operation}( entryDto: $entryDto) {{
                            success
                            errorMessage
                            serializedData
                          }}
                        }}
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { entryDto }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            return response.Data;
        }

        public async Task<RequestResult> PromoteAsync(RecordType recordType, string entityId)
        {
            var operation = "promote";
            var request = new GraphQLRequest
            {
                Query = $@"
                        mutation {operation}($recordTypeName: String!,  $entityId: String!) {{
                          {operation}(recordTypeName: $recordTypeName,  entityId: $entityId) {{
                            success
                            errorMessage
                            serializedData
                          }}
                        }}
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { recordTypeName = recordType.ToString(), entityId }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<RequestResult>(request, operation);
            return response.Data;
        }

        public RequestService(IGraphQlClient graphQLRequestSender)
        {
            _graphQLRequestSender = graphQLRequestSender;
        }
        private IGraphQlClient _graphQLRequestSender;
    }
}
