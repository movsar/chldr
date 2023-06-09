
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_tools;
using chldr_data.remote.SqlEntities;
using chldr_data.remote.Services;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.remote.Repositories
{
    internal class SqlLanguagesRepository : SqlRepository<LanguageModel, LanguageDto>, ILanguagesRepository
    {
        public SqlLanguagesRepository(SqlContext context, string _userId) : base(context, _userId) { }

        protected override RecordType RecordType => RecordType.Language;

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }

        public override LanguageModel Get(string entityId)
        {
            throw new NotImplementedException();
        }


        public List<LanguageModel> GetAllLanguages()
        {
            throw new NotImplementedException();
        }

        public override void Insert(LanguageDto dto)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LanguageModel> Take(int limit)
        {
            throw new NotImplementedException();
        }

        public override void Update(LanguageDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
