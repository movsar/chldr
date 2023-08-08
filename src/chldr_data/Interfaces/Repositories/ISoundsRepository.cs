using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface ISoundsRepository : IRepository<PronunciationModel, PronunciationDto>
    {
        Task<ChangeSetModel> Promote(ISound sound);
    }
}
