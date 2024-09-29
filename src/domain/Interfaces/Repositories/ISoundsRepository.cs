using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;

namespace domain.Interfaces.Repositories
{
    public interface ISoundsRepository : IRepository<SoundModel, SoundDto>
    {
        Task<ChangeSetModel> Promote(ISound sound);
        Task RemoveRange(string[] sounds);
    }
}
