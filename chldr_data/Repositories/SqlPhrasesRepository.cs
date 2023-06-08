using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class SqlPhrasesRepository : SqlRepository<SqlPhrase, PhraseModel, PhraseDto>
    {
        public SqlPhrasesRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType => RecordType.Phrase;

        public override async Task Insert(string userId, PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public override PhraseModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(string userId, PhraseDto dto)
        {
            throw new NotImplementedException();
        }
    }
}