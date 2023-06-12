using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_tools;

namespace chldr_data.remote.Repositories
{
    internal class SqlSourcesRepository : SqlRepository<SqlSource, SourceModel, SourceDto>, ISourcesRepository
    {
        protected override RecordType RecordType => RecordType.Source;
        public SqlSourcesRepository(SqlContext context, string _userId) : base(context, _userId) { }

        public override SourceModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SourceModel> Take(int limit)
        {
            throw new NotImplementedException();
        }

        public override void Update(SourceDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Add(SourceDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
