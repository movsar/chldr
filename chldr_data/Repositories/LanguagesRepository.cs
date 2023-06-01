
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.Repositories
{
    public class LanguagesRepository : ILanguageRepository
    {
        public Task AddAsync(LanguageDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(LanguageDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LanguageModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LanguageModel> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(LanguageDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
