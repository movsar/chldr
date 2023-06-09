using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_tools;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.remote.Repositories
{
    public class SqlPhrasesRepository : SqlRepository<PhraseModel, PhraseDto>, IPhrasesRepository
    {
        public SqlPhrasesRepository(SqlContext context, string _userId) : base(context, _userId) { }

        protected override RecordType RecordType => RecordType.Phrase;

        public override PhraseModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public EntryModel GetByEntryId(string entryId)
        {
            throw new NotImplementedException();
        }

        public void Update(EntryDto updatedEntryDto, ITranslationsRepository translationsRepository)
        {
            throw new NotImplementedException();
        }

        public override void Update(PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Insert(PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}