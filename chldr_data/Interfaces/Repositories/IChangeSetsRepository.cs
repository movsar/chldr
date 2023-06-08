using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IChangeSetsRepository : IRepository<ChangeSetModel, ChangeSetDto>
    {
        void AddRange(IEnumerable<ChangeSetDto> dtos);
        IEnumerable<ChangeSetModel> Get(string[] changeSetIds);
        IEnumerable<ChangeSetModel> GetLatest(int minIndex);
    }
}
