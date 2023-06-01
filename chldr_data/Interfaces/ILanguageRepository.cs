using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;

namespace chldr_data.Interfaces
{
    public interface ILanguageRepository : IRepository<SqlLanguage, LanguageModel, LanguageDto>
    {
    }
}
