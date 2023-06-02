using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Interfaces;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class ChangeSetsRepository : Repository<SqlChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        public ChangeSetsRepository(SqlContext sqlContext) : base(sqlContext) { }

        public override void Add(ChangeSetDto dto)
        {
            var changeSet = SqlChangeSet.FromDto(dto);
            SqlContext.Add(changeSet);
        }

        public override ChangeSetModel Get(string entityId)
        {
            var changeSet = SqlContext.Find<SqlChangeSet>(entityId);
            return ChangeSetModel.FromEntity(changeSet);
        }

        public override IEnumerable<ChangeSetModel> GetAll()
        {
            return SqlContext.ChangeSets.Select(c => ChangeSetModel.FromEntity(c));
        }

        public override void Update(ChangeSetDto dto)
        {
            var changeSet = SqlContext.Find<SqlChangeSet>(dto.ChangeSetId);
            if (changeSet == null) {
                throw new NullReferenceException();
            }

            changeSet.RecordChanges = dto.RecordChanges;
            changeSet.RecordId = dto.RecordId;
            changeSet.CreatedAt = dto.CreatedAt;
            changeSet.RecordType = (int)dto.RecordType;
            changeSet.Operation = (int)dto.Operation;
            changeSet.UserId = dto.UserId;

            SqlContext.Update(changeSet);
        }
    }
}
