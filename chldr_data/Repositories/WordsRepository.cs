using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Models.Words;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class WordsRepository : EntriesRepository<WordModel>
    {
        public WordsRepository(IDataAccess dataAccess) : base(dataAccess) { }

        public WordModel GetById(string entityId)
        {
            var word = Database.All<SqlWord>().FirstOrDefault(w => w.WordId == entityId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return new WordModel(word.Entry);
        }

        public List<WordModel> GetRandomWords(int limit)
        {
            var words = Database.All<SqlWord>().AsEnumerable().Take(limit);
            return words.Select(w => new WordModel(w.Entry)).ToList();
        }

        public string Insert(WordDto newWord)
        {
            if (!string.IsNullOrEmpty(newWord.EntryId))
            {
                throw new InvalidOperationException();
            }

            var source = Database.Find<SqlSource>(new ObjectId(newWord.SourceId));

            // Initialize an entry object
            var entry = new SqlEntry()
            {
                Rate = Convert.ToInt32(newWord.Rate),
                Source = source,
            };

            // Insert data
            var word = new SqlWord()
            {
                Entry = entry,
                Content = newWord.Content,
                Notes = newWord.Notes
            };

            entry.Type = (int)EntryType.Word;
            //entry.Word = word;

            foreach (var translationDto in newWord.Translations)
            {
                var language = Database.All<SqlLanguage>().FirstOrDefault(t => t.Code == translationDto.LanguageCode);
                if (language == null)
                {
                    throw new Exception("Language cannot be empty");
                }

                entry.Translations.Add(new SqlTranslation()
                {
                    Entry = entry,
                    Language = language,
                    Content = translationDto.Content,
                    Notes = translationDto.Notes
                });
            }

            Database.Write(() =>
            {
                Database.Add(entry);
            });

            return entry.EntryId;
        }

        public void Update(UserModel user, WordDto wordDto)
        {
            var word = Database.Find<SqlWord>(new ObjectId(wordDto.WordId));
            Database.Write(() =>
            {
                word.Entry.Rate = user.GetRateRange().Lower;
                word.Entry.RawContents = word.Content.ToLower();
                foreach (var translationDto in wordDto.Translations)
                {
                    var translationId = new ObjectId(translationDto.TranslationId);
                    SqlTranslation translation = Database.Find<SqlTranslation>(translationId);
                    if (translation == null)
                    {
                        translation = new SqlTranslation()
                        {
                            Entry = word.Entry,
                            Language = Database.All<SqlLanguage>().First(l => l.Code == translationDto.LanguageCode),
                        };
                    }
                    translation.Rate = user.GetRateRange().Lower;
                    translation.Content = translationDto.Content;
                    translation.Notes = translationDto.Notes;
                    translation.RawContents = translation.GetRawContents();
                }
                word.PartOfSpeech = (int)wordDto.PartOfSpeech;
                word.Content = wordDto.Content;
                //foreach (var grammaticalClass in wordDto.GrammaticalClasses)
                //{
                //    word.GrammaticalClasses.Add(grammaticalClass);
                //}
                word.Notes = wordDto.Notes;
            });

            OnEntryUpdated(new WordModel(word.Entry));
        }
    }
}
