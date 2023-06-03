using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;

namespace chldr_data.Interfaces
{
    public interface IChangeSetsRepository : IRepository<SqlChangeSet, ChangeSetModel, ChangeSetDto>
    {
        void AddRange(IEnumerable<ChangeSetDto> dtos);
        IEnumerable<ChangeSetModel> Get(string[] changeSetIds);
        IEnumerable<ChangeSetModel> GetLatest(int limit);
    }
}
