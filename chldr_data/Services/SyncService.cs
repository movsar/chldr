using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Services
{
    internal class SyncService
    {
        //public string Insert(WordDto newWord)
        //{
        //    if (!string.IsNullOrEmpty(newWord.EntryId))
        //    {
        //        throw new InvalidOperationException();
        //    }

        //    var source = Database.Find<RealmSource>(newWord.SourceId);

        //    // Initialize an entry object
        //    var entry = new RealmEntry()
        //    {
        //        Rate = Convert.ToInt32(newWord.Rate),
        //        Source = source,
        //    };

        //    // Insert data
        //    var word = new RealmWord()
        //    {
        //        Entry = entry,
        //        Content = newWord.Content,
        //        Notes = newWord.Notes
        //    };

        //    entry.Type = (int)EntryType.Word;
        //    entry.Word = word;

        //    foreach (var translationDto in newWord.Translations)
        //    {
        //        var language = Database.All<RealmLanguage>().FirstOrDefault(t => t.Code == translationDto.LanguageCode);
        //        if (language == null)
        //        {
        //            throw new Exception("Language cannot be empty");
        //        }

        //        entry.Translations.Add(new RealmTranslation()
        //        {
        //            Entry = entry,
        //            Language = language,
        //            Content = translationDto.Content,
        //            Notes = translationDto.Notes
        //        });
        //    }

        //    Database.Write(() =>
        //    {
        //        Database.Add(entry);
        //    });

        //    return entry.EntryId;
        //}
        //public IDataAccess DataAccess { get; }
        //public Realm Database => DataAccess.GetDatabase();
        //public Repository(IDataAccess dataAccess)
        //{
        //    DataAccess = dataAccess;
        //}
        //protected void SetPropertyValue(object obj, string propertyName, object value)
        //{
        //    var propertyInfo = obj.GetType().GetProperty(propertyName);
        //    if (propertyInfo != null)
        //    {
        //        propertyInfo.SetValue(obj, value);
        //    }
        //}

        //protected async Task Sync(List<ChangeSetModel>? changeSets = null)
        //{
        //    var changeSetsToApply = changeSets;
        //    if (changeSetsToApply == null)
        //    {
        //        // TODO: Get latest changesets based on...?
        //    }

        //    Database.Write(() => {


        //        foreach (var changeSet in changeSetsToApply)
        //        {
        //            var changes = JsonConvert.DeserializeObject<List<Change>>(changeSet.RecordChanges);
        //            if (changes == null || changes.Count == 0)
        //            {
        //                continue;
        //            }

        //            // Apply changes to the local database
        //            if (changeSet.RecordType == Enums.RecordType.Word)
        //            {
        //                try
        //                {
        //                    var realmWord = Database.Find<RealmWord>(changeSet.RecordId);
        //                    if (realmWord == null)
        //                    {
        //                        throw new NullReferenceException();
        //                    }

        //                    foreach (var change in changes)
        //                    {
        //                        SetPropertyValue(realmWord, change.Property, change.NewValue);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {

        //                }
        //            }
        //        }

        //    });

        //public void Delete(string Id)
        //{
        //    var entry = Database.Find<RealmEntry>(Id);
        //    if (entry == null)
        //    {
        //        return;
        //    }

        //    Database.Write(() =>
        //    {
        //        foreach (var translation in entry.Translations)
        //        {
        //            Database.Remove(translation);
        //        }
        //        switch ((EntryType)entry.Type)
        //        {
        //            case EntryType.Word:
        //                Database.Remove(entry.Word!);
        //                break;
        //            case EntryType.Phrase:
        //                Database.Remove(entry.Phrase!);
        //                break;
        //            case EntryType.Text:
        //                Database.Remove(entry.Text!);
        //                break;
        //            default:
        //                break;
        //        }
        //        Database.Remove(entry);
        //    });
        //}

        //        var word = Database.Find<RealmWord>(new ObjectId(wordDto.WordId));
        //        Database.Write(() =>
        //            {
        //                //word.Entry.Rate = loggedInUser.GetRateRange().Lower;
        //                word.Entry.RawContents = word.Content.ToLower();
        //                foreach (var translationDto in wordDto.Translations)
        //                {
        //                    var translationId = new ObjectId(translationDto.TranslationId);
        //        RealmTranslation translation = Database.Find<RealmTranslation>(translationId);
        //                    if (translation == null)
        //                    {
        //                        translation = new RealmTranslation()
        //        {
        //            Entry = word.Entry,
        //                            Language = Database.All<RealmLanguage>().First(l => l.Code == translationDto.LanguageCode),
        //                        };
        //    }
        //    //translation.Rate = loggedInUser.GetRateRange().Lower;
        //    translation.Content = translationDto.Content;
        //                    translation.Notes = translationDto.Notes;
        //                    translation.RawContents = translation.GetRawContents();
        //                }
        //word.PartOfSpeech = (int)wordDto.PartOfSpeech;
        //word.Content = wordDto.Content;
        ////foreach (var grammaticalClass in wordDto.GrammaticalClasses)
        ////{
        ////    word.GrammaticalClasses.Add(grammaticalClass);
        ////}
        //word.Notes = wordDto.Notes;
        //            });
    }
    }
}
