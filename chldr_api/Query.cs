using chldr_data.Entities;
using chldr_tools;

namespace chldr_api
{
    public class Query
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<SqlEntry> GetEntries([Service] SqlContext context) => context.Entries;
    }
}
