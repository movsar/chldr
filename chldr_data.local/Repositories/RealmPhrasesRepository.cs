using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_tools;
using chldr_data.local.RealmEntities;
using Realms;
using chldr_data.DatabaseObjects.Interfaces;
using System.Linq;

namespace chldr_data.Repositories
{
    public class RealmPhrasesRepository : RealmRepository<RealmPhrase, PhraseModel, PhraseDto>
    {
        public RealmPhrasesRepository(Realm context) : base(context) { }

        protected override RecordType RecordType => RecordType.Phrase;
        public static PhraseModel FromEntity(RealmPhrase phrase)
        {
            return PhraseModel.FromEntity(phrase.Entry,
                                    phrase.Entry.Phrase,
                                    phrase.Entry.Source,
                                    phrase.Entry.Translations
                                        .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        }
        public PhraseModel GetByEntryId(string entryId)
        {
            var word = DbContext.Find<RealmEntry>(entryId)!.Phrase;
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }
        public override IEnumerable<ChangeSetModel> Add(string userId, PhraseDto dto)
        {
            throw new NotImplementedException();
        }

        public override PhraseModel Get(string entityId)
        {
            var word = DbContext.All<RealmPhrase>().FirstOrDefault(w => w.PhraseId == entityId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, PhraseDto dto)
        {
            throw new NotImplementedException();
        }
    }
}