using GraphQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public interface IGraphQLRequestSender
    {
        Task<GraphQLResponse<T>> SendRequestAsync<T>(GraphQLRequest request, string mutation);
    }
}
