using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using chldr_utils.Interfaces;
using chldr_data;
using Newtonsoft.Json;
using chldr_data.Models;

namespace chldr_utils.Services
{
    public class GraphQLClient : IGraphQlClient
    {
        private readonly GraphQLHttpClient _graphQLClient;
        private readonly ExceptionHandler _exceptionHandler;

        public GraphQLClient(ExceptionHandler exceptionHandler, EnvironmentService environmentService)
        {
            var apiHost = environmentService.IsDevelopment ? Constants.DevApiHost : Constants.ProdApiHost;

            _graphQLClient = new GraphQLHttpClient($"{apiHost}/graphql", new NewtonsoftJsonSerializer());
            _exceptionHandler = exceptionHandler;
        }

        public async Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation)
        {
            try
            {
                var response = await _graphQLClient.SendMutationAsync<JObject>(request);
                if (response.Errors?.Length > 0)
                {
                    throw new Exception(response.Errors![0].Message);
                }

                return new GraphQLResponse<T>
                {
                    Data = response.Data[mutation]!.ToObject<T>()!,
                    Errors = response.Errors,
                };
            }
            catch (GraphQLHttpRequestException graphQlException)
            {
                var errorResponse = JsonConvert.DeserializeObject<GraphQlErrorResponse>(graphQlException.Content);

                // Now you can access the deserialized data
                foreach (var error in errorResponse.Errors)
                {
                    var messageParts = new List<string>();

                    var extensions = error.Extensions.Select(e => e.Value).ToArray();

                    if (extensions.Count() == 0)
                    {
                        messageParts.Add(error.Message);
                    }
                    else if (extensions.Count() > 0)
                    {
                        messageParts.Add(extensions[0]);
                    }
                    else if (extensions.Count() > 1)
                    {
                        _exceptionHandler.LogError(extensions[1]);
                    }

                    var errorMessage = string.Join(", ", messageParts);

                    throw _exceptionHandler.Error(errorMessage);
                }

                throw new Exception("An unhandled error occurred");
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogError(ex.Message);

                if (ex.Message.Contains("connection"))
                {
                    throw new Exception("Network error occurred", ex);
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
