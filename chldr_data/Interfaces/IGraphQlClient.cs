using GraphQL;

namespace chldr_utils.Interfaces
{
    public interface IGraphQlClient
    {
        Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation);
    }
}
