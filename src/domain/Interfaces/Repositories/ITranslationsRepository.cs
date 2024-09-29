using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using domain.Models;

namespace domain.Interfaces.Repositories
{
    public interface ITranslationsRepository : IRepository<TranslationModel, TranslationDto>
    {      
        Task<ChangeSetModel> Promote(ITranslation translation);
        Task RemoveRange(string[] translations);
    }
}
