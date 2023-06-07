using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class RealmPhrasesRepository : RealmRepository<SqlPhrase, PhraseModel, PhraseDto>
    {
        public RealmPhrasesRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType => RecordType.Phrase;

        public override IEnumerable<ChangeSetModel> Add(string userId, PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public override PhraseModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, PhraseDto dto)
        {
            throw new NotImplementedException();
        }
    }
}