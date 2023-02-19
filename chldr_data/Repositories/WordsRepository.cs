using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class WordsRepository : EntriesRepository<WordModel>
    {
        public WordsRepository(IRealmServiceFactory realmServiceFactory) : base(realmServiceFactory) { }

        public WordModel GetById(ObjectId entityId)
        {
            var word = Database.All<Word>().FirstOrDefault(w => w._id == entityId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return new WordModel(word);
        }

        public ObjectId Insert(WordDto newWord)
        {
            return new ObjectId();
        }

        public void Update(UserModel user, WordDto wordDto)
        {
            var word = Database.Find<Word>(new ObjectId(wordDto.WordId));
            Database.Write(() =>
            {
                word.Entry.Rate = user.GetRateRange().Lower;
                word.Entry.RawContents = word.Content.ToLower();
                foreach (var translationDto in wordDto.Translations)
                {
                    var translationId = new ObjectId(translationDto.TranslationId);
                    Translation translation = Database.Find<Translation>(translationId);
                    if (translation == null)
                    {
                        translation = new Translation()
                        {
                            Entry = word.Entry,
                            Language = Database.All<Language>().First(l => l.Code == translationDto.LanguageCode),
                        };
                    }
                    translation.Rate = user.GetRateRange().Lower;
                    translation.Content = translationDto.Content;
                    translation.Notes = translationDto.Notes;
                    translation.RawContents = translation.GetRawContents();
                }
                word.PartOfSpeech = (int)wordDto.PartOfSpeech;
                word.Content = wordDto.Content;
                foreach (var grammaticalClass in wordDto.GrammaticalClasses)
                {
                    word.GrammaticalClasses.Add(grammaticalClass);
                }
                word.Notes = wordDto.Notes;
            });

            OnEntryUpdated(new WordModel(word));
        }
    }
}
