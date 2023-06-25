using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_tools;
using chldr_utils.Services;

namespace chldr_data.remote.Repositories
{
    internal class SqlSourcesRepository : SqlRepository<SqlSource, SourceModel, SourceDto>, ISourcesRepository
    {
        protected override RecordType RecordType => RecordType.Source;
        public SqlSourcesRepository(SqlContext context, FileService fileService, string _userId) : base(context, fileService, _userId) { }

        public override void Update(SourceDto dto)
        {
            var source = SqlSource.FromDto(dto);
            _dbContext.Update(source);
        }

        public override void Add(SourceDto dto)
        {
            var source = SqlSource.FromDto(dto);
            _dbContext.Add(source);
        }

        protected override SourceModel FromEntityShortcut(SqlSource entity)
        {
            return SourceModel.FromEntity(entity);
        }
    }
}
