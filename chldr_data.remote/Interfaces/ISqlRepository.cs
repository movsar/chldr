using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.remote.Interfaces
{
    public interface ISqlRepository<TModel, TDto> : IRepository<TModel, TDto>
    {
        public List<ChangeSetModel> Add(TDto dto);
        public List<ChangeSetModel> Update(TDto dto);
        public List<ChangeSetModel> Remove(string removedId);

        public List<ChangeSetModel> AddRange(IEnumerable<TDto> added);
        public List<ChangeSetModel> UpdateRange(IEnumerable<TDto> updated);
        public List<ChangeSetModel> RemoveRange(IEnumerable<string> removedIds);
    }
}
