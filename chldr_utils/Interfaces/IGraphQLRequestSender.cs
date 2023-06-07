using GraphQL;

namespace chldr_utils.Interfaces
{
    public interface IGraphQLRequestSender
    {
        Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation);
    }
}
