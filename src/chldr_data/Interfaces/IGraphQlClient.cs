using GraphQL;

namespace chldr_data.Interfaces
{
    public interface IGraphQlClient
    {
        Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation);
    }
}
