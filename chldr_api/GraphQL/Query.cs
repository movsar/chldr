using chldr_data.Entities;
using chldr_tools;
using HotChocolate.Authorization;

namespace chldr_api
{
    public class Query
    {
        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<SqlEntry> GetEntries([Service] SqlContext context, int limit) => context.Entries.Take(limit);        
    }
}