using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IChangeSetsRepository : IRepository<ChangeSetModel, ChangeSetDto>
    {
        void Add(IChangeSetEntity changeSetEntity);
        void AddRange(IEnumerable<IChangeSetEntity> changeSetEntities);
        IEnumerable<ChangeSetModel> Get(string[] changeSetIds);
        Task<List<ChangeSetModel>> TakeLastAsync(int count);
    }
}
