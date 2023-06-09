using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.local.RealmEntities;
using Realms;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;

namespace chldr_data.Repositories
{
    public class RealmPhrasesRepository : RealmRepository<RealmPhrase, PhraseModel, PhraseDto>, IPhrasesRepository
    {
        public RealmPhrasesRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        protected override RecordType RecordType => RecordType.Phrase;
        public static PhraseModel FromEntity(RealmPhrase phrase)
        {
            return PhraseModel.FromEntity(phrase.Entry,
                                    phrase.Entry.Phrase,
                                    phrase.Entry.Source,
                                    phrase.Entry.Translations
                                        .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        }
        public EntryModel GetByEntryId(string entryId)
        {
            var word = _dbContext.Find<RealmEntry>(entryId)!.Phrase;
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }
        public override void Insert( PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public override PhraseModel Get(string entityId)
        {
            var word = _dbContext.All<RealmPhrase>().FirstOrDefault(w => w.PhraseId == entityId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }

        public override void Update(PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Delete( string entityId)
        {
            throw new NotImplementedException();
        }

        public void Update(EntryDto updatedEntryDto, ITranslationsRepository translationsRepository)
        {
            throw new NotImplementedException();
        }
    }
}