using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.local.RealmEntities;

namespace chldr_data.Readers
{
    public class PhrasesReader : DataReader<RealmPhrase, PhraseModel>
    {
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
            var word = Database.Find<RealmEntry>(entryId)!.Phrase;
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }

        public PhraseModel GetById(string phraseId)
        {
            var word = Database.All<RealmPhrase>().FirstOrDefault(w => w.PhraseId == phraseId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }
    }
}
