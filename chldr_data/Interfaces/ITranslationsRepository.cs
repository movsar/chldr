using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces
{
    public interface ITranslationsRepository
    {
        IEnumerable<ChangeSetModel> Add(string userId, TranslationDto dto);
        TranslationModel Get(string entityId);
        IEnumerable<ChangeSetModel> Update(string userId, TranslationDto translationDto);
    }
}
