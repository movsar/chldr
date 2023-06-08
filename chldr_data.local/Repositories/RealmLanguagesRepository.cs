
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using Realms;
using chldr_data.local.RealmEntities;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;

namespace chldr_data.Repositories
{
    public class RealmLanguagesRepository : RealmRepository<RealmLanguage, LanguageModel, LanguageDto>, ILanguagesRepository
    {
        public RealmLanguagesRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        protected override RecordType RecordType => RecordType.Language;

        public override async Task Add(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }
        public List<LanguageModel> GetAllLanguages()
        {
            var languages = _dbContext.All<RealmLanguage>().AsEnumerable().Select(l => LanguageModel.FromEntity(l));
            return languages.ToList();
        }

        public override LanguageModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(string userId, LanguageDto dto)
        {
            throw new NotImplementedException();
        }

        public override Task Delete(string userId, string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
