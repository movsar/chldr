using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;

namespace chldr_data.Interfaces
{
    public interface IWordsRepository : IRepository<SqlWord, WordModel, WordDto>
    {
    }
}
