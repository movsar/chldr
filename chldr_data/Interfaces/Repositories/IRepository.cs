using chldr_data.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        IEnumerable<TModel> Take(int limit);
        TModel Get(string entityId);
        void Insert(TDto dto);
        void Update(TDto dto);
        void Delete(string entityId);
    }
}