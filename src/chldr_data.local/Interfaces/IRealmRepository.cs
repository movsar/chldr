using chldr_data.Interfaces.Repositories;

namespace chldr_data.local.Interfaces
{
    public interface IRealmRepository<TModel, TDto> : IRepository<TModel, TDto>
    {
    }
}