using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.local.Interfaces
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