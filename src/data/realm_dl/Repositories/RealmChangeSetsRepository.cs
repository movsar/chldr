using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;
using domain;
using domain.Interfaces.Repositories;
using realm_dl.RealmEntities;
using chldr_utils;
using Realms;
using chldr_utils.Services;
using domain.Interfaces;
using domain.Enums;

namespace realm_dl.Repositories
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task<List<ChangeSetModel>> Update(ChangeSetDto dto)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task<List<ChangeSetModel>> Add(ChangeSetDto dto)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<List<ChangeSetModel>> TakeLastAsync(int count)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
