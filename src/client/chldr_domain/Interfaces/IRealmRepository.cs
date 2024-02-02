using core.DatabaseObjects.Models;
using core.Interfaces.Repositories;

namespace chldr_domain.Interfaces
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