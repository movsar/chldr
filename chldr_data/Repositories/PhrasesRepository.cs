using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.Repositories
{
    public class PhrasesRepository : IPhraseRepository
    {
        public Task AddAsync(PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PhraseModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PhraseModel> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(PhraseDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
}
