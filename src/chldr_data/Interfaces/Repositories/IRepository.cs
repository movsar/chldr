using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        Task<IEnumerable<TModel>> TakeAsync(int offset, int limit);
        Task<List<TModel>> GetRandomsAsync(int limit);

        Task<TModel> Get(string entityId);

        Task<List<ChangeSetModel>> Add(TDto dto);
        Task<List<ChangeSetModel>> Update(TDto dto);
        Task<List<ChangeSetModel>> Remove(string entityId);

        Task<List<ChangeSetModel>> AddRange(IEnumerable<TDto> added);
        Task<List<ChangeSetModel>> UpdateRange(IEnumerable<TDto> updated);
        Task<List<ChangeSetModel>> RemoveRange(IEnumerable<string> removed);
    }
}