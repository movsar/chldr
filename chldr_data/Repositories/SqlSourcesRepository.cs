using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class SqlSourcesRepository : SqlRepository<SqlSource, SourceModel, SourceDto>
    {
        protected override RecordType RecordType => RecordType.Source;
        public SqlSourcesRepository(SqlContext context) : base(context) { }

        public override async Task Add(string userId, SourceDto dto)
        {
            throw new NotImplementedException();
        }

        public override SourceModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(string userId, SourceDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
