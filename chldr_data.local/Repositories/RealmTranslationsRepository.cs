using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using chldr_data.Enums;
using chldr_data.Services;
using chldr_data.local.RealmEntities;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmTranslationsRepository : RealmRepository<RealmTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public RealmTranslationsRepository(Realm context) : base(context) { }

        protected override RecordType RecordType => RecordType.Translation;

        public override IEnumerable<ChangeSetModel> Add(string userId, TranslationDto dto)
        {
            throw new NotImplementedException();
        }

        public override TranslationModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, TranslationDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
