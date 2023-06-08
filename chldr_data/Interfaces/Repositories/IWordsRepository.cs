using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;

namespace chldr_data.Interfaces.Repositories
{
    public interface IWordsRepository : IRepository<WordModel, WordDto>, IEntriesRepository
    { }
}
