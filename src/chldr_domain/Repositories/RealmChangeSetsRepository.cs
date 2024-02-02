using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Models;
using core.Enums;
using core.Interfaces.Repositories;
using chldr_domain.RealmEntities;
using chldr_utils;
using Realms;
using chldr_utils.Services;
using core.Interfaces;

namespace core.Repositories
{
    public class RealmChangeSetsRepository : RealmRepository<RealmChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        public RealmChangeSetsRepository(IExceptionHandler exceptionHandler, IFileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.ChangeSet;
        public override ChangeSetModel FromEntity(RealmChangeSet entity)
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

        public override async Task<List<ChangeSetModel>> Update(ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override async Task<List<ChangeSetModel>> Add(ChangeSetDto dto)
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
            var models = _dbContext.All<RealmChangeSet>()
                .OrderByDescending(c => c.ChangeSetIndex)
                .TakeLast(count)
                .ToList();

            return models.AsEnumerable().Select(ChangeSetModel.FromEntity).ToList();
        }

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
