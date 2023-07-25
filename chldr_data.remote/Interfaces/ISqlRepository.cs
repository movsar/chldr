using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.remote.Interfaces
{
    public interface ISqlRepository<TModel, TDto> : IRepository<TModel, TDto>
    {
    }
}
