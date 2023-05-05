using chldr_data.Interfaces;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using chldr_data.ResponseTypes;

namespace chldr_data.Services
{
    public class GraphQLRequestSender : IGraphQLRequestSender
    {
        private readonly GraphQLHttpClient _graphQLClient;
        public GraphQLRequestSender()
        {
            string appHost = "https://localhost:7065/graphql/";
            _graphQLClient = new GraphQLHttpClient($"{appHost}/graphql/", new NewtonsoftJsonSerializer());
        }

        public async Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation)
        {
            try
            {
                var response = await _graphQLClient.SendMutationAsync<JObject>(request);
                var deserializedResponse = new GraphQLResponse<T>
                {

                    Data = response.Data[mutation]!.ToObject<T>(),
                    Errors = response.Errors,
                };
                return deserializedResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while sending GraphQL request", ex);
            }
        }
    }
}
