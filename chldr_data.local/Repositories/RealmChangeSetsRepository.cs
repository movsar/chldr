using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_tools;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmChangeSetsRepository : RealmRepository<RealmChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        protected override RecordType RecordType => throw new Exception("Shouldn't be used");

        public RealmChangeSetsRepository(Realm sqlContext) : base(sqlContext) { }

        public override ChangeSetModel Get(string entityId)
        {
            var changeSet = DbContext.Find<RealmChangeSet>(entityId);
            return ChangeSetModel.FromEntity(changeSet);
        }

        public void AddRange(IEnumerable<ChangeSetDto> dtos)
        {
            var entities = dtos.Select(d => (RealmChangeSet)RealmChangeSet.FromDto(d));
            foreach (var entity in entities)
            {
                DbContext.Add(entity);
            }
        }

        public IEnumerable<ChangeSetModel> GetLatest(int limit)
        {
            var models = DbContext.All<RealmChangeSet>()
                .OrderByDescending(c => c.ChangeSetIndex)
                .Take(limit)
                .Select(c => (ChangeSetModel)ChangeSetModel.FromEntity(c));

            return models;
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            var models = DbContext.All<RealmChangeSet>()
                .Where(c => changeSetIds.Contains(c.ChangeSetId))
                .Select(c => ChangeSetModel.FromEntity(c));

            return models;
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override IEnumerable<ChangeSetModel> Add(string userId, ChangeSetDto dto)
        {
            var changeSet = (RealmChangeSet)RealmChangeSet.FromDto(dto);
            DbContext.Add(changeSet);

            return EmptyResult;
        }
    }
}
