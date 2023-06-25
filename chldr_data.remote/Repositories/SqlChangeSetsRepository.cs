using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Realms;

namespace chldr_data.remote.Repositories
{
    internal class SqlChangeSetsRepository : SqlRepository<SqlChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        protected override RecordType RecordType => throw new Exception("Shouldn't be used");

        public SqlChangeSetsRepository(SqlContext sqlContext, FileService fileService, string _userId) : base(sqlContext, fileService, _userId) { }

        protected override ChangeSetModel FromEntityShortcut(SqlChangeSet entity)
        {
            return ChangeSetModel.FromEntity(entity);
        }

        public async Task<List<ChangeSetModel>> TakeLastAsync(int count)
        {
            var lastChangeSetIndices = await _dbContext.ChangeSets
            .OrderByDescending(c => c.ChangeSetIndex)
            .Select(c => c.ChangeSetIndex)
            .Take(count)
            .ToListAsync();

            var models = await _dbContext.ChangeSets
                .Where(c => lastChangeSetIndices.Contains(c.ChangeSetIndex))
                .ToListAsync();

            return models.Select(ChangeSetModel.FromEntity).ToList();
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            var models = _dbContext.ChangeSets
                .Where(c => changeSetIds.Contains(c.ChangeSetId))
                .Select(c => ChangeSetModel.FromEntity(c));

            return models;
        }

        public override void Update(ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override void Add(ChangeSetDto dto)
        {
            var changeSet = SqlChangeSet.FromDto(dto);
            _dbContext.Add(changeSet);
        }
    }
}
