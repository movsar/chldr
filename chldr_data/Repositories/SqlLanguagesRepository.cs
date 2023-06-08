
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class SqlLanguagesRepository : SqlRepository<SqlLanguage, LanguageModel, LanguageDto>
    {
        public SqlLanguagesRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType => RecordType.Language;

        public override async Task Insert(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }

        public override LanguageModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
