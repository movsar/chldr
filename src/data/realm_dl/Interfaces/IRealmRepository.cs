using domain.DatabaseObjects.Models;
using domain.Interfaces.Repositories;

namespace realm_dl.Interfaces
{
    public interface IRealmRepository<TModel, TDto> : IRepository<TModel, TDto>
    {
        Task Add(TDto dto);
        Task Update(TDto dto);
        Task Remove(string entityId);

        Task AddRange(IEnumerable<TDto> added);
        Task UpdateRange(IEnumerable<TDto> updated);
        Task RemoveRange(IEnumerable<string> removed);
    }
}