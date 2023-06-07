using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class RealmChangeSetsRepository : RealmRepository<SqlChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        protected override RecordType RecordType => throw new Exception("Shouldn't be used");

        public RealmChangeSetsRepository(SqlContext sqlContext) : base(sqlContext) { }

        public override ChangeSetModel Get(string entityId)
        {
            var changeSet = _context.Find<SqlChangeSet>(entityId);
            return ChangeSetModel.FromEntity(changeSet);
        }

        public void AddRange(IEnumerable<ChangeSetDto> dtos)
        {
            var entities = dtos.Select(d => (SqlChangeSet)SqlChangeSet.FromDto(d));
            _context.AddRange(entities);
        }

        public IEnumerable<ChangeSetModel> GetLatest(int limit)
        {
            var models = _context.ChangeSets
                .OrderByDescending(c => c.ChangeSetIndex)
                .Take(limit)
                .Select(c => (ChangeSetModel)ChangeSetModel.FromEntity(c));

            return models;
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            var models = _context.ChangeSets
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
            var changeSet = SqlChangeSet.FromDto(dto);
            _context.Add(changeSet);

            return EmptyResult;
        }
    }
}
