using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {

        Task<IEnumerable<TModel>> TakeAsync(int offset, int limit);
        List<TModel> GetRandoms(int limit);

        TModel Get(string entityId);
        void Add(TDto dto);
        void Update(TDto dto);
        void Remove(string entityId);

        void AddRange(IEnumerable<TDto> added);
        void UpdateRange(IEnumerable<TDto> updated);
        void RemoveRange(IEnumerable<string> removed);
    }
}