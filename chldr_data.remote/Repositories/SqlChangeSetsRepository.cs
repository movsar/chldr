using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_tools;
using chldr_utils.Services;

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
        public void AddRange(IEnumerable<ChangeSetDto> dtos)
        {
            var entities = dtos.Select(d => (SqlChangeSet)SqlChangeSet.FromDto(d));
            _dbContext.AddRange(entities);
        }

        public IEnumerable<ChangeSetModel> GetLatest(int limit)
        {
            var models = _dbContext.ChangeSets
                .OrderByDescending(c => c.ChangeSetIndex)
                .Take(limit)
                .Select(c => (ChangeSetModel)ChangeSetModel.FromEntity(c));

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
