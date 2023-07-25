using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        Task<IEnumerable<TModel>> TakeAsync(int offset, int limit);
        Task<List<TModel>> GetRandomsAsync(int limit);

        TModel Get(string entityId);
        List<ChangeSetModel> Add(TDto dto);
        List<ChangeSetModel> Update(TDto dto);
        List<ChangeSetModel> Remove(string entityId);

        List<ChangeSetModel> AddRange(IEnumerable<TDto> added);
        List<ChangeSetModel> UpdateRange(IEnumerable<TDto> updated);
        List<ChangeSetModel> RemoveRange(IEnumerable<string> removed);
    }
}