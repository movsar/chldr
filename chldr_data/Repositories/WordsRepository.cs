using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_utils;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Realms.Sync.MongoClient;

namespace chldr_data.Repositories
{
    public class WordsRepository : EntriesRepository<WordModel>
    {
        public WordsRepository(IRealmService realmService) : base(realmService) { }


        public WordModel GetById(ObjectId entityId)
        {
            return new WordModel(Database.All<Word>().FirstOrDefault(w => w._id == entityId));
        }

        public void Insert(WordDto newWord)
        {
            Database.Write(() =>
            {
                var allForms = newWord.Forms.Union(newWord.NounDeclensions.Values).Union(newWord.VerbTenses.Values).ToList();
                allForms.Remove(newWord.Content);
                allForms.Add(newWord.Content);

                Entry entry = new Entry()
                {
                    Type = (int)EntryType.Word,
                    // TODO: User = findBy(_userStore.CurrentUserInfo()),
                    Rate = newWord.Rate,
                    RawContents = string.Join("; ", allForms.Select(w => w)).ToLower(),
                    // TODO: Source = 
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

                newWord.Translations.Select(t => )

            });
        }
    }
}
