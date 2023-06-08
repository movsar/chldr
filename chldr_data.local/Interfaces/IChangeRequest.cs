using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces
{
    internal interface IChangeRequest
    {
        Task<IEnumerable<ChangeSetModel>> Add(string userId, WordDto dto);
        Task<IEnumerable<ChangeSetModel>> Update(string userId, WordDto dto);
        Task<IEnumerable<ChangeSetModel>> Delete(string userId, string entityId);
    }
}
