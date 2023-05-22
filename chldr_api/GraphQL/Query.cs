﻿using chldr_data.DatabaseObjects.SqlEntities;
using chldr_tools;
using HotChocolate.Authorization;

namespace chldr_api
{
    public class Query
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<SqlEntry> GetEntries([Service] SqlContext context, int limit) => context.Entries.Take(limit);        
    }
}