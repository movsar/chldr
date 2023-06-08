using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
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
        protected override RecordType RecordType => throw new Exception("Shouldn't be used");

        public RealmChangeSetsRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        public override ChangeSetModel Get(string entityId)
        {
            var changeSet = _dbContext.Find<RealmChangeSet>(entityId);
            return ChangeSetModel.FromEntity(changeSet);
        }

        public void AddRange(IEnumerable<ChangeSetDto> dtos)
        {
            var entities = dtos.Select(d => (RealmChangeSet)RealmChangeSet.FromDto(d));
            foreach (var entity in entities)
            {
                _dbContext.Add(entity);
            }
        }

        public IEnumerable<ChangeSetModel> GetLatest(int limit)
        {
            var models = _dbContext.All<RealmChangeSet>()
                .OrderByDescending(c => c.ChangeSetIndex)
                .Take(limit)
                .Select(c => (ChangeSetModel)ChangeSetModel.FromEntity(c));

            return models;
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            var models = _dbContext.All<RealmChangeSet>()
                .Where(c => changeSetIds.Contains(c.ChangeSetId))
                .Select(c => ChangeSetModel.FromEntity(c));

            return models;
        }

        public override async Task Update(string userId, ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override async Task Add(string userId, ChangeSetDto dto)
        {
            var changeSet = (RealmChangeSet)RealmChangeSet.FromDto(dto);
            _dbContext.Add(changeSet);

            //return EmptyResult;
        }

        public override Task Delete(string userId, string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
