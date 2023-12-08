using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface ITranslationsRepository : IRepository<TranslationModel, TranslationDto>
    {      
        Task<ChangeSetModel> Promote(ITranslation translation);
    }
}
