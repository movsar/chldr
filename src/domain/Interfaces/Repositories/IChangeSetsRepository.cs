using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;

namespace domain.Interfaces.Repositories
{
    public interface IChangeSetsRepository : IRepository<ChangeSetModel, ChangeSetDto>
    {
        Task AddRange(IEnumerable<ChangeSetDto> changeSets);
        IEnumerable<ChangeSetModel> Get(string[] changeSetIds);
        Task<List<ChangeSetModel>> TakeLastAsync(int count);
    }
}
