
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_tools;
using Realms;
using chldr_data.local.RealmEntities;

namespace chldr_data.Repositories
{
    public class RealmLanguagesRepository : RealmRepository<RealmLanguage, LanguageModel, LanguageDto>
    {
        public RealmLanguagesRepository(Realm context) : base(context) { }

        protected override RecordType RecordType => RecordType.Language;

        public override IEnumerable<ChangeSetModel> Add(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }

        public override LanguageModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
