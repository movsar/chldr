using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_tools;
using chldr_utils.Interfaces;
using chldr_utils;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmChangeSetsRepository : RealmRepository<RealmChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        public RealmChangeSetsRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }
        protected override RecordType RecordType => throw new Exception("Shouldn't be used");
        protected override ChangeSetModel FromEntityShortcut(RealmChangeSet entity)
        {
            return ChangeSetModel.FromEntity(entity);
        }

        public IEnumerable<ChangeSetModel> GetLatest(int limit)
        {
            var models = _dbContext.All<RealmChangeSet>()
                .OrderByDescending(c => c.ChangeSetIndex)
                .Take(limit)
                .Select(c => ChangeSetModel.FromEntity(c));

            return models;
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            var models = _dbContext.All<RealmChangeSet>()
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
            // Store max 500 changesets
            _dbContext.Write(() =>
            {
                var changeSet = (RealmChangeSet)RealmChangeSet.FromDto(dto);
                _dbContext.Add(changeSet);
            });
        }
    }
}
