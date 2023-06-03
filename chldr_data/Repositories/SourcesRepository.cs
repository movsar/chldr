﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class SourcesRepository : Repository<SqlSource, SourceModel, SourceDto>
    {
        public SourcesRepository(SqlContext context) : base(context) { }

        public override void Add(SourceDto dto)
        {
            throw new NotImplementedException();
        }

        public override SourceModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override void Update(SourceDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
