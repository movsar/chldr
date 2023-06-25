using chldr_data.Interfaces.Repositories;

namespace chldr_data.local.Interfaces
{
    public interface IRealmRepository<TModel, TDto> : IRepository<TModel, TDto>
    {
        public void Add(TDto dto);
        public void Update(TDto dto);
        public void Remove(string entityId);
        public void AddRange(IEnumerable<TDto> added);
        public void UpdateRange(IEnumerable<TDto> updated);
        public void RemoveRange(IEnumerable<string> removedIds);
    }
}