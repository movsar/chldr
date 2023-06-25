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
using Microsoft.IdentityModel.Abstractions;
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
          

            List<long> lastChangeSetIndices = null!;
            try
            {

                lastChangeSetIndices = await _dbContext.ChangeSets
                    .OrderByDescending(c => c.ChangeSetIndex)
                    .Select(c => c.ChangeSetIndex)
                    .Take(count)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                // 1. An item with the same key has already been added. Key: server=104.248.40.142;port=3306;..
                // 2. Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.
                throw;
            }

            List<SqlChangeSet> entities = null!;
            try
            {
                entities = await _dbContext.ChangeSets
                   .Where(c => lastChangeSetIndices.Contains(c.ChangeSetIndex))
                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            List<ChangeSetModel> models = null!;

            try
            {
                models = entities.Select(ChangeSetModel.FromEntity).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            return models;
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
