using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class SourcesRepository : Repository<SqlSource, SourceModel, SourceDto>
    {
        protected override RecordType RecordType => RecordType.Source;
        public SourcesRepository(SqlContext context) : base(context) { }

        public override IEnumerable<ChangeSetModel> Add(string userId, SourceDto dto)
        {
            throw new NotImplementedException();
        }

        public override SourceModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, SourceDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
