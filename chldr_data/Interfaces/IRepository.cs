using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces
{
    public interface IRepository<TEntity, TModel, TDto>
    {
        event Action<EntryModel>? EntryUpdated;
        event Action<EntryModel>? EntryInserted;
        event Action<EntryModel>? EntryDeleted;
        event Action<EntryModel>? EntryAdded;

        TModel Get(string entityId);
        IEnumerable<ChangeSetModel> Add(string userId, TDto dto);
        IEnumerable<ChangeSetModel> Update(string userId, TDto dto);
        IEnumerable<ChangeSetModel> Delete(string userId, string translationId);
    }
}