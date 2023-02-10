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
            try
            {
                var allForms = newWord.Forms.Union(newWord.NounDeclensions.Values).Union(newWord.VerbTenses.Values).ToList();
                allForms.Remove(newWord.Content);
                allForms.Add(newWord.Content);

                Entry entry = new Entry()
                {
                    // TODO: User = findBy(_userStore.CurrentUserInfo()),
                    Rate = newWord.Rate,
                    RawContents = string.Join("; ", allForms.Select(w => w)).ToLower(),
                    // TODO: Source = 
                    Source = Database.Find<Source>(new ObjectId(newWord.SourceId))
                };

                Word wordEntity = new Word()
                {
                    Entry = entry,
                    Content = newWord.Content,
                    Forms = string.Join(";", newWord.Forms),
                    NounDeclensions = Word.StringifyNounDeclensions(newWord.NounDeclensions),
                    VerbTenses = Word.StringifyVerbTenses(newWord.VerbTenses),
                    GrammaticalClass = newWord.GrammaticalClass,
                    Notes = newWord.Notes,
                    PartOfSpeech = (int)newWord.PartOfSpeech,
                };

                foreach (var translationDto in newWord.Translations)
                {
                    var translationEntity = new Translation()
                    {
                        Entry = entry,
                        Language = Database.All<Language>().First(l => l.Code == translationDto.LanguageCode),
                        Notes = translationDto.Notes,
                        Content = translationDto.Content,
                        // TODO: Rate = 
                        RawContents = translationDto.Content.ToLower(),
                        // TODO: User = 
                    };

                    entry.Translations.Add(translationEntity);
                }

                entry.Type = (int)EntryType.Word;
                entry.Word = wordEntity;

                Database.Write(() =>
                {
                    Database.Add(entry);
                });

                return entry.Word._id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(WordDto wordDto)
        {
            var word = Database.Find<Word>(new ObjectId(wordDto.WordId));
            Database.Write(() =>
            {

                //word.Entry.Rate = 
                word.Entry.RawContents = word.GetRawContents();
                foreach (var translationDto in wordDto.Translations)
                {
                    Translation translation = Database.Find<Translation>(translationDto.TranslationId);
                    if (translation == null)
                    {
                        translation = new Translation()
                        {
                            Entry = word.Entry,
                            Language = Database.All<Language>().First(l => l.Code == translationDto.LanguageCode),
                            //Rate = User.rate
                        };
                    }
                    //translation.Rate = 
                    translation.Content = translationDto.Content;
                    translation.Notes = translationDto.Notes;
                    translation.RawContents = translation.GetRawContents();
                }

                word.Content = wordDto.Content;
                word.GrammaticalClass = wordDto.GrammaticalClass;
                word.Notes = wordDto.Notes;
                word.NounDeclensions = Word.StringifyNounDeclensions(wordDto.NounDeclensions);
                word.VerbTenses = Word.StringifyNounDeclensions(wordDto.VerbTenses);
                word.Forms = Word.StringifyForms(wordDto.Forms);

            });
        }
    }
}
