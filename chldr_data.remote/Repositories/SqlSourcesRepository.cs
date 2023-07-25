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
    public class SqlSourcesRepository : SqlRepository<SqlSource, SourceModel, SourceDto>, ISourcesRepository
    {
        protected override RecordType RecordType => RecordType.Source;
        public SqlSourcesRepository(SqlContext context, FileService fileService, string _userId) : base(context, fileService, _userId) { }

        public override async Task<List<ChangeSetModel>> Update(SourceDto dto, string userId)
        {
            var source = SqlSource.FromDto(dto);
            _dbContext.Update(source);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.SourceId);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Add(SourceDto dto, string userId)
        {
            var source = SqlSource.FromDto(dto);
            _dbContext.Add(source);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.SourceId);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        protected override SourceModel FromEntityShortcut(SqlSource entity)
        {
            return SourceModel.FromEntity(entity);
        }
    }
}
