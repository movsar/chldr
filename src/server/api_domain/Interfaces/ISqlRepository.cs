using core.DatabaseObjects.Models;
using core.Interfaces.Repositories;

namespace api_domain.Interfaces
{
    public interface ISqlRepository<TModel, TDto> : IRepository<TModel, TDto>
    {
        Task<List<ChangeSetModel>> AddAsync(TDto dto);
        Task<List<ChangeSetModel>> UpdateAsync(TDto dto);
        Task<List<ChangeSetModel>> RemoveAsync(string entityId);

        Task<List<ChangeSetModel>> AddRange(IEnumerable<TDto> added);
        Task<List<ChangeSetModel>> UpdateRange(IEnumerable<TDto> updated);
        Task<List<ChangeSetModel>> RemoveRange(IEnumerable<string> removed);
    }
}
