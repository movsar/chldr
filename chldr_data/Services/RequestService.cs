using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
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


        public async Task<IEnumerable<ChangeSetDto>> GetChangeSets()
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        query retrieveLatestChangeSets($minIndex: Int!) {
                          retrieveLatestChangeSets(minIndex: $minIndex) {
                            changeSetId                   
                            changeSetIndex
                            recordId
                            recordChanges
                            recordType
                            operation
                            userId
                          }
                        }
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { minIndex = 0 }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<IEnumerable<ChangeSetDto>>(request, "retrieveLatestChangeSets");
            return response.Data;
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
                throw new Exception("Error:unexpected_error");
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
