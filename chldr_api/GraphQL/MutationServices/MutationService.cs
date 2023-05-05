using chldr_tools;

namespace chldr_api.GraphQL.MutationServices
{
    public class MutationService
    {
        protected readonly SqlContext dbContext;

        public MutationService()
        {
            dbContext = new SqlContext();
        }
    }
}
