﻿using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using chldr_utils.Interfaces;
using chldr_data;
using Newtonsoft.Json;

namespace chldr_utils.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class Location
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
        public List<Location> Locations { get; set; }
        public List<string> Path { get; set; }
        public Dictionary<string, string> Extensions { get; set; }
    }

    public class ErrorResponse
    {
        public List<Error> Errors { get; set; }
    }
    public class GraphQLClient : IGraphQlClient
    {
        private readonly GraphQLHttpClient _graphQLClient;
        private readonly ExceptionHandler _exceptionHandler;

        public GraphQLClient(ExceptionHandler exceptionHandler, EnvironmentService environmentService)
        {
            _graphQLClient = new GraphQLHttpClient($"{Constants.DevApiHost}/graphql", new NewtonsoftJsonSerializer());
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
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(graphQlException.Content);

                // Now you can access the deserialized data
                foreach (var error in errorResponse.Errors)
                {
                    var messageParts = new List<string>();
                    messageParts.Add(error.Message);

                    var extensions = error.Extensions.Select(e => e.Value).ToArray();

                    if (extensions.Count() > 0)
                    {
                        messageParts.Add(extensions[0]);
                    }
                    if (extensions.Count() > 1)
                    {
                        _exceptionHandler.LogError(extensions[1]);
                    }

                    var errorMessage = string.Join(", ", messageParts);

                    Console.WriteLine(errorMessage);
                    Console.WriteLine(messageParts[1]);

                    _exceptionHandler.LogError(errorMessage);

                    throw new Exception(errorMessage);
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
                    throw new Exception("Unexpected error occurred", ex);
                }
            }
        }
    }
}
