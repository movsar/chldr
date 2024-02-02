using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;

namespace core.Interfaces.Repositories
{
    public interface IPronunciationsRepository : IRepository<PronunciationModel, PronunciationDto>
    {
        Task<ChangeSetModel> Promote(IPronunciation sound);
        Task RemoveRange(string[] sounds);
    }
}
