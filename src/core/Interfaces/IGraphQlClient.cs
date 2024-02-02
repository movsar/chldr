using GraphQL;

namespace core.Interfaces
{
    public interface IGraphQlClient
    {
        Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation);
    }
}
