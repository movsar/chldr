using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IEntriesRepository
    {
        public EntryModel GetByEntryId(string entryId);
        void Update(EntryDto updatedEntryDto, ITranslationsRepository translationsRepository);
    }
}
