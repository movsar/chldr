using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.remote.Repositories
{
    public class SqlSourcesRepository : SqlRepository<SqlSource, SourceModel, SourceDto>, ISourcesRepository
    {
        protected override RecordType RecordType => RecordType.Source;
        public SqlSourcesRepository(DbContextOptions<SqlContext> dbConfig, FileService fileService, string _userId) : base(dbConfig, fileService, _userId) { }
        public override async Task<List<SourceModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlSource>().Select(e => e.SourceId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlSource>()
              .Where(e => randomlySelectedIds.Contains(e.SourceId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(FromEntityShortcut).ToList();
            return models;
        }
        public override async Task<List<ChangeSetModel>> Update(SourceDto dto)
        {
            var source = SqlSource.FromDto(dto);
            _dbContext.Update(source);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.SourceId);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Add(SourceDto dto)
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
