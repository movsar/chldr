using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;
using core.Models;

namespace core.Interfaces.Repositories
{
    public interface ITranslationsRepository : IRepository<TranslationModel, TranslationDto>
    {      
        Task<ChangeSetModel> Promote(ITranslation translation);
        Task RemoveRange(string[] translations);
    }
}
