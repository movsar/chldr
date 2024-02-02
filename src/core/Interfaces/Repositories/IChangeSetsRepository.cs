using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;

namespace core.Interfaces.Repositories
{
    public interface IChangeSetsRepository : IRepository<ChangeSetModel, ChangeSetDto>
    {
        Task AddRange(IEnumerable<ChangeSetDto> changeSets);
        IEnumerable<ChangeSetModel> Get(string[] changeSetIds);
        Task<List<ChangeSetModel>> TakeLastAsync(int count);
    }
}
