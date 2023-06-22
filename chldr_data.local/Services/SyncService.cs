using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_data.Services;
using chldr_utils.Interfaces;
using GraphQL;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

namespace chldr_data.local.Services
{
    public class SyncService
    {
        private readonly RequestService _requestService;
        private Timer _timer;

        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1);
        public SyncService(RequestService requestService)
        {
            _requestService = requestService;
        }
        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        private Realm _dbContext => Realm.GetInstance(RealmDataProvider.OfflineDatabaseConfiguration);

        public string Insert(SourceDto sourceDto)
        {
            if (!string.IsNullOrEmpty(sourceDto.SourceId))
            {
                throw new InvalidOperationException();
            }

            var source = new RealmSource()
            {
                Name = sourceDto.Name,
                Notes = sourceDto.Notes
            };

            _dbContext.Write(() =>
            {
                _dbContext.Add(source);
            });

            return source.SourceId;
        }
        //public string Insert(EntryDto newWord)
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
        bool _isRunning = false;
        private async Task PullRemoteDatabase()
        {
            var users = await _requestService.TakeUsers(0, 100);

            // Get users
            // Get sources
            // Get entries with sounds and translations
            // Get latest changeset

            // Get by chunks until finished
        }
        internal async Task Sync()
        {
            if (_isRunning)
            {
                return;
            }

            try
            {
                _isRunning = true;
                await _syncLock.WaitAsync();

                var latestChangeSet = _dbContext.All<RealmChangeSet>().LastOrDefault();
                var latestChangeSetIndex = latestChangeSet == null ? 0 : latestChangeSet.ChangeSetIndex;

                if (latestChangeSetIndex == 0)
                {
                    await PullRemoteDatabase();
                    return;
                }

             
                var changeSetsToApply = await _requestService.GetChangeSets();
                if (changeSetsToApply == null)
                {
                    // TODO: Get latest changesets based on...?
                }
                return;
                _dbContext.Write(() =>
                {


                    foreach (var changeSet in changeSetsToApply)
                    {
                        var changes = JsonConvert.DeserializeObject<List<Change>>(changeSet.RecordChanges);
                        if (changes == null || changes.Count == 0)
                        {
                            continue;
                        }

                        // Apply changes to the local database
                        if (changeSet.RecordType == Enums.RecordType.Entry)
                        {
                            try
                            {
                                var realmWord = _dbContext.Find<RealmEntry>(changeSet.RecordId);
                                if (realmWord == null)
                                {
                                    throw new NullReferenceException();
                                }

                                foreach (var change in changes)
                                {
                                    SetPropertyValue(realmWord, change.Property, change.NewValue);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                });

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

                //        var word = Database.Find<RealmWord>(new ObjectId(EntryDto.WordId));
                //        Database.Write(() =>
                //            {
                //                //word.Entry.Rate = loggedInUser.GetRateRange().Lower;
                //                word.Entry.RawContents = word.Content.ToLower();
                //                foreach (var translationDto in EntryDto.Translations)
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
                //word.PartOfSpeech = (int)EntryDto.PartOfSpeech;
                //word.Content = EntryDto.Content;
                ////foreach (var grammaticalClass in EntryDto.GrammaticalClasses)
                ////{
                ////    word.GrammaticalClasses.Add(grammaticalClass);
                ////}
                //word.Notes = EntryDto.Notes;
                //            });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                _syncLock.Release();
                _isRunning = false;
            }

        }

        internal void BeginListening()
        {
            _timer = new Timer(async state => await Sync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Task.Run(Sync);
        }
    }
}