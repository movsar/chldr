using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        IEnumerable<TModel> Take(int limit);
        TModel Get(string entityId);
        void Add(TDto dto);
        void Update(TDto dto);
        void Remove(string entityId);
        void AddRange(IEnumerable<TDto> added);
        void UpdateRange(IEnumerable<TDto> updated);
        void RemoveRange(IEnumerable<string> removed);
    }
}