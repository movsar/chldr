using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;
using sql_dl;
using sql_dl.Interfaces;
using domain.Interfaces.Repositories;
using sql_dl.Services;
using sql_dl.SqlEntities;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using sql_dl;
using domain.Interfaces;
using domain.Enums;

namespace sql_dl.Repositories
{
    public class SqlChangeSetsRepository : SqlRepository<SqlChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        protected override RecordType RecordType => throw new Exception("Shouldn't be used");
        public SqlChangeSetsRepository(SqlContext context, IFileService fileService, string _userId) : base(context, fileService, _userId) { }

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
            entities = await _dbContext.ChangeSets
               .Where(c => lastChangeSetIndices.Contains(c.ChangeSetIndex))
               .ToListAsync();


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

        public override async Task<List<ChangeSetModel>> UpdateAsync(ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override async Task<List<ChangeSetModel>> AddAsync(ChangeSetDto dto)
        {
            var changeSet = SqlChangeSet.FromDto(dto);
            await _dbContext.AddAsync(changeSet);

            return new List<ChangeSetModel>();
        }

        public override Task<List<ChangeSetModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        public override async Task<ChangeSetModel> GetAsync(string entityId)
        {
            var changeSet = await _dbContext.ChangeSets.FirstOrDefaultAsync(t => t.ChangeSetId!.Equals(entityId));
            if (changeSet == null)
            {
                throw new NullReferenceException();
            }

            return ChangeSetModel.FromEntity(changeSet);
        }

        public override async Task<List<ChangeSetModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.ChangeSets
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return entities.Select(ChangeSetModel.FromEntity).ToList();
        }

        Task IChangeSetsRepository.AddRange(IEnumerable<ChangeSetDto> changeSets)
        {
            throw new NotImplementedException();
        }
    }
}
