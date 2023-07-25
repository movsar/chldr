using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_tools;
using chldr_utils;
using Realms;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Repositories
{
    public class RealmChangeSetsRepository : RealmRepository<RealmChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        public RealmChangeSetsRepository(ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.ChangeSet;
        protected override ChangeSetModel FromEntityShortcut(RealmChangeSet entity)
        {
            return ChangeSetModel.FromEntity(entity);
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            var models = _dbContext.All<RealmChangeSet>()
                .Where(c => changeSetIds.Contains(c.ChangeSetId))
                .Select(c => ChangeSetModel.FromEntity(c));

            return models;
        }

        public override List<ChangeSetModel> Update(ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override List<ChangeSetModel> Add(ChangeSetDto dto)
        {
            // Store max 500 changesets
            _dbContext.Write(() =>
            {
                var changeSet = RealmChangeSet.FromDto(dto, _dbContext);

                _dbContext.Add(changeSet);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public async Task<List<ChangeSetModel>> TakeLastAsync(int count)
        {
            var models = await _dbContext.All<RealmChangeSet>()
                .OrderByDescending(c => c.ChangeSetIndex)
                .TakeLast(count)
                .ToListAsync();

            return models.AsEnumerable().Select(ChangeSetModel.FromEntity).ToList();
        }
    }
}
