using GraphQL;

namespace domain.Interfaces
{
    public interface IGraphQlClient
    {
        Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation);
    }
}
