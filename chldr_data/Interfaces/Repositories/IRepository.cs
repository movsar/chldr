using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        Task<IEnumerable<TModel>> TakeAsync(int offset, int limit);
        Task<List<TModel>> GetRandoms(int limit);

        TModel Get(string entityId);
    }
}