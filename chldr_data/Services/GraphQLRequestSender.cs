﻿using chldr_data.Interfaces;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using chldr_data.ResponseTypes;
using chldr_utils;

namespace chldr_data.Services
{
    public class GraphQLRequestSender : IGraphQLRequestSender
    {
        private readonly GraphQLHttpClient _graphQLClient;
        private readonly ExceptionHandler _exceptionHandler;

        public GraphQLRequestSender(ExceptionHandler exceptionHandler)
        {
            string appHost = "https://localhost:7065/graphql/";
            _graphQLClient = new GraphQLHttpClient($"{appHost}/graphql/", new NewtonsoftJsonSerializer());
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
            catch (GraphQLHttpRequestException graphQlException){
                _exceptionHandler.LogError(graphQlException.Content);
                throw new Exception("Unexpected error occurred while sending GraphQL request", graphQlException);
            }
            catch (Exception ex)
            {                
                _exceptionHandler.LogError(ex.Message);
                throw new Exception("Unexpected error occurred", ex);
            }
        }
    }
}
