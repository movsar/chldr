﻿using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using chldr_utils.Interfaces;
using chldr_data;

namespace chldr_utils.Services
{
    public class GraphQLClient : IGraphQlClient
    {
        private readonly GraphQLHttpClient _graphQLClient;
        private readonly ExceptionHandler _exceptionHandler;

        public GraphQLClient(ExceptionHandler exceptionHandler, EnvironmentService environmentService)
        {
            _graphQLClient = new GraphQLHttpClient($"{Constants.ProdApiHost}/graphql", new NewtonsoftJsonSerializer());
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
                _exceptionHandler.LogError(graphQlException.Content!);

                if (graphQlException.Content!.Contains("Could not save changes"))
                {
                    throw new Exception("Could not save changes", graphQlException);
                }
                else
                {
                    throw new Exception("An error occurred while sending the request", graphQlException);
                }
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
                    throw new Exception("Unexpected error occurred", ex);
                }
            }
        }
    }
}
