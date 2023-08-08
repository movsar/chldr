using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.remote.Interfaces
{
    public interface ISqlRepository<TModel, TDto> : IRepository<TModel, TDto>
    {
        Task<List<ChangeSetModel>> Add(TDto dto);
        Task<List<ChangeSetModel>> Update(TDto dto);
        Task<List<ChangeSetModel>> Remove(string entityId);

        Task<List<ChangeSetModel>> AddRange(IEnumerable<TDto> added);
        Task<List<ChangeSetModel>> UpdateRange(IEnumerable<TDto> updated);
        Task<List<ChangeSetModel>> RemoveRange(IEnumerable<string> removed);
    }
}
