using Data.Entities;
using Realms;
using System.Collections.Immutable;
using System.Diagnostics;
using MongoDB.Bson;
using Data.Models;
using Data.Search;
using Data.Enums;
using Data.Interfaces;
using Data.Services.PartialMethods;
using Realms.Sync;

namespace Data.Services
{
    public class RealmDataAccessService : IDataAccessService
    {

        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;
        private Realm _syncedRealm;
        Realm RealmDatabase = null;
        // By using previous results we increase search speed when a user is typing

        private void SetRatesForOfficialDictionaryEntries()
        {
            RealmDatabase.Write(() =>
            {
                Debug.WriteLine("Doing stuff");

                foreach (var entry in RealmDatabase.All<EntryEntity>().AsEnumerable().Where(entry =>
                    entry.Source.Name == "Maciev"
                    || entry.Source.Name == "Anatslovar"
                    || entry.Source.Name == "Malaev"
                    || entry.Source.Name == "Karasaev"
                ))
                {
                    entry.Rate = 5;
                    foreach (var translation in entry.Translations)
                    {
                        translation.Rate = 5;
                    }
                }
            });
        }
        private void UpdateEntryRawContentField()
        {
            // This must be done either automatically or manually from time to time to improve search speed
            RealmDatabase.Write(() =>
            {
                Debug.WriteLine("Doing stuff");

                foreach (var entry in RealmDatabase.All<EntryEntity>())
                {
                    switch (entry.Type)
                    {
                        case EntryType.Word:
                            var allForms = WordEntity.GetAllUniqueWordForms(entry.Word.Content, entry.Word.Forms, entry.Word.NounDeclensions, entry.Word.VerbTenses);
                            entry.RawContents = string.Join("; ", allForms.Select(w => w)).ToLower();

                            break;

                        case EntryType.Phrase:
                            entry.RawContents = entry.Phrase.Content.ToLower();
                            break;
                    }
                }

                foreach (var translation in RealmDatabase.All<TranslationEntity>())
                {
                    translation.RawContents = translation.Content.ToLower();
                }
            });
        }
        private void UpdateFormsField()
        {
            // This must be done either automatically or manually from time to time
            RealmDatabase.Write(() =>
            {
                Debug.WriteLine("Doing stuff");
                foreach (var entry in RealmDatabase.All<EntryEntity>())
                {
                    if (entry.Type != EntryType.Word)
                    {
                        continue;
                    }

                    var forms = entry.Word?.Forms;
                    entry.Word.Forms = forms.Replace(",", ";");
                }

            });
        }
        private void DeclensionsAndTensesToLower()
        {
            RealmDatabase.Write(() =>
            {
                Debug.WriteLine("Doing stuff");
                foreach (var entry in RealmDatabase.All<EntryEntity>())
                {
                    if (entry.Type != EntryType.Word)
                    {
                        continue;
                    }

                    entry.Word.NounDeclensions = entry.Word.NounDeclensions.ToLower();
                    entry.Word.VerbTenses = entry.Word.VerbTenses.ToLower();
                }
            });
        }
        private void AddSources()
        {
            RealmDatabase.Write(() =>
            {
                List<SourceEntity> sources = new List<SourceEntity>
                {
                    new SourceEntity(){ Name = "GEO"},
                    new SourceEntity(){ Name = "Yurslovar"},
                };

                foreach (var s in sources)
                {
                    RealmDatabase.Add(s);
                }

            });
        }
        private void ImportPhrases()
        {
            //var legacyPhrases = LegacyEntriesProvider.GetLegacyPhraseEntries();
            //var legacyTranslations = LegacyEntriesProvider.GetLegacyPhraseTranslationEntries();

            //var yurslovarSource = RealmDatabase.All<SourceEntity>().Where(source => source.Name.Equals("Yurslovar")).First();
            //var geoSource = RealmDatabase.All<SourceEntity>().Where(source => source.Name.Equals("GEO")).First();
            //var userSource = RealmDatabase.All<SourceEntity>().Where(source => source.Name.Equals("User")).First();

            //var adminUser = RealmDatabase.All<UserEntity>().First();

            //var langs = legacyTranslations.DistinctBy(t => t.LanguageCode);

            //int counter = 0;
            //RealmDatabase.Write(() =>
            //{
            //    foreach (var phraseEntry in legacyPhrases)
            //    {
            //        // Find or insert the source
            //        SourceEntity source = null;
            //        if (string.IsNullOrWhiteSpace(phraseEntry.Source))
            //        {
            //            source = RealmDatabase.All<SourceEntity>().FirstOrDefault(source => source.Name.Equals(phraseEntry.UserId.ToString()));
            //            if (source == null)
            //            {
            //                source = new SourceEntity() { Name = phraseEntry.UserId.ToString(), Notes = "Imported from legacy database" };
            //            }
            //        }
            //        else
            //        {
            //            source = RealmDatabase.All<SourceEntity>().FirstOrDefault(source => source.Name.Equals(phraseEntry.Source, StringComparison.OrdinalIgnoreCase));
            //            if (source == null)
            //            {
            //                source = new SourceEntity() { Name = phraseEntry.Source, Notes = "Imported from legacy database" };
            //            }
            //        }

            //        // Initialize an entry object
            //        var entry = new EntryEntity()
            //        {
            //            User = adminUser,
            //            Rate = Convert.ToInt32(phraseEntry.Rate),
            //            Source = source,
            //        };

            //        // Insert data
            //        switch (phraseEntry.Type.ToLower())
            //        {
            //            case "word":
            //                var word = new WordEntity()
            //                {
            //                    Entry = entry,
            //                    Content = phraseEntry.Phrase,
            //                    Forms = phraseEntry.Forms,
            //                    Notes = phraseEntry.Notes
            //                };

            //                RealmDatabase.Add(word);

            //                entry.Type = EntryType.Word;
            //                entry.Word = word;

            //                break;
            //            case "phrase":
            //                var phrase = new PhraseEntity()
            //                {
            //                    Entry = entry,
            //                    Content = phraseEntry.Phrase,
            //                    Notes = phraseEntry.Notes
            //                };

            //                RealmDatabase.Add(phrase);

            //                entry.Type = EntryType.Phrase;
            //                entry.Phrase = phrase;
            //                break;
            //            default:
            //                break;
            //        }

            //        RealmDatabase.Add(entry);

            //        // Insert translations
            //        foreach (var t in legacyTranslations.Where(t => t.PhraseId == phraseEntry.Id))
            //        {
            //            var translation = new TranslationEntity()
            //            {
            //                Content = t.Translation,
            //                Language = RealmDatabase.All<LanguageEntity>().First(l => l.Code == t.LanguageCode),
            //                Entry = entry,
            //                Rate = Convert.ToInt32(t.Rate),
            //                User = adminUser,
            //            };

            //            RealmDatabase.Add(translation);
            //            entry.Translations.Add(translation);
            //        }

            //        counter++;
            //    }
            //});

            //Debug.WriteLine($"Added {counter} entries");
        }
        private void SetTranslationEntryAndUserLinks()
        {
            var adminUser = RealmDatabase.All<UserEntity>().First();

            RealmDatabase.Write(() =>
            {
                foreach (var entry in RealmDatabase.All<EntryEntity>())
                {
                    entry.User = adminUser;

                    foreach (var translation in entry.Translations)
                    {
                        translation.User = adminUser;
                        translation.Entry = entry;
                    }
                }
            });
        }
        private void RemoveDuplicates()
        {
            int counter = 0;
            var translations = RealmDatabase.All<TranslationEntity>();
            //translations.AsEnumerable().Select(translation => new { Translation = translation.Content, LanguageCode = translation.Language.Code });
            var entries = RealmDatabase.All<EntryEntity>();

            var distinctEntryIds = entries.AsEnumerable().DistinctBy(e => e.RawContents).Select(e => e._id).ToArray();

            var duplicatingEntryIds = new List<ObjectId>();
            foreach (var entry in entries)
            {
                if (!distinctEntryIds.Contains(entry._id))
                {
                    duplicatingEntryIds.Add(entry._id);
                }
            }

            var entriesToRemove = new HashSet<EntryEntity>();
            foreach (var entry in entries.AsEnumerable().Where(e => duplicatingEntryIds.Contains(e._id)))
            {
                foreach (var translation in entry.Translations)
                {
                    if (translations.Count(t => t.Content == translation.Content && t.Language == translation.Language) > 1)
                    {
                        entriesToRemove.Add(translation.Entry);
                    }
                }
            }

            RealmDatabase.Write(() =>
            {
                foreach (var entry in entriesToRemove)
                {
                    switch (entry.Type)
                    {
                        case EntryType.Word:
                            RealmDatabase.Remove(entry.Word);
                            break;
                        case EntryType.Phrase:
                            RealmDatabase.Remove(entry.Phrase);
                            break;
                        case EntryType.Text:
                            RealmDatabase.Remove(entry.Text);
                            break;
                    }

                    foreach (var translation in entry.Translations)
                    {
                        RealmDatabase.Remove(translation);
                    }

                    RealmDatabase.Remove(entry);
                    counter++;
                }
            });

            Debug.WriteLine($"Removed {counter} duplicates");
        }
        private void SetSourceNotes()
        {
            RealmDatabase.Write(() =>
            {
                var source = RealmDatabase.All<SourceEntity>().First(s => s.Name == "1");
                source.Name = "ikhasakhanov";
                source.Notes = "";
            });
        }
        private void RemoveWeirdos()
        {
            RealmDatabase.Write(() =>
            {
                var thing = "Ψ".ToLower();
                var phrases = RealmDatabase.All<PhraseEntity>().Where(p => p.Content.Contains("Ψ") || p.Notes.Contains("Ψ")).ToList();
                var words = RealmDatabase.All<WordEntity>().Where(w => w.Notes.Contains("Ψ")).ToList();

                foreach (var phrase in phrases)
                {
                    if (phrase.Notes.Length < 6)
                    {
                        phrase.Notes = null;
                        continue;
                    }

                    var translationNotes = phrase.Notes.Split("Ψ");
                    foreach (var translationNote in translationNotes)
                    {
                        if (string.IsNullOrWhiteSpace(translationNote))
                        {
                            phrase.Notes = null;
                            continue;
                        }
                        var parts = translationNote.Split(":");
                        var languageCode = parts[0];
                        var note = parts[1];

                        var translation = phrase.Entry.Translations.FirstOrDefault(t => t.Language.Code == languageCode);
                        if (translation == null)
                        {
                            phrase.Notes = note;
                        }
                        else
                        {
                            translation.Notes = note;
                            phrase.Notes = null;
                        }
                    }
                }

                foreach (var word in words)
                {
                    if (word.Notes.Length < 6)
                    {
                        word.Notes = null;
                        continue;
                    }
                    var translationNotes = word.Notes.Split("Ψ");
                    foreach (var translationNote in translationNotes)
                    {
                        if (string.IsNullOrWhiteSpace(translationNote))
                        {
                            word.Notes = null;
                            continue;
                        }
                        var parts = translationNote.Split(":");
                        var languageCode = parts[0];
                        var note = parts[1];

                        var translation = word.Entry.Translations.FirstOrDefault(t => t.Language.Code == languageCode);
                        if (translation == null)
                        {
                            word.Notes = note;
                        }
                        else
                        {
                            translation.Notes = note;
                            word.Notes = null;
                        }
                    }
                }
            });
        }
        private void CopyObjectIds()
        {
            RealmDatabase.Write(() =>
            {
                var entries = RealmDatabase.All<EntryEntity>();
                foreach (var entity in entries)
                {
                    entity._id = entity._id;
                }

                var translations = RealmDatabase.All<TranslationEntity>();
                foreach (var entity in translations)
                {
                    entity._id = entity._id;
                }

                var languages = RealmDatabase.All<LanguageEntity>();
                foreach (var entity in languages)
                {
                    entity._id = entity._id;
                }

                var phrases = RealmDatabase.All<PhraseEntity>();
                foreach (var entity in phrases)
                {
                    entity._id = entity._id;
                }

                var users = RealmDatabase.All<UserEntity>();
                foreach (var entity in users)
                {
                    entity._id = entity._id;
                }

                var words = RealmDatabase.All<WordEntity>();
                foreach (var entity in words)
                {
                    entity._id = entity._id;
                }

                var sources = RealmDatabase.All<SourceEntity>();
                foreach (var entity in sources)
                {
                    entity._id = entity._id;
                }
            });
        }

        private void AddSubscriptionsToRealm()
        {
            var subscriptions = RealmDatabase.Subscriptions;
            subscriptions.Update(() =>
            {
                subscriptions.Add(RealmDatabase.All<EntryEntity>());
                subscriptions.Add(RealmDatabase.All<WordEntity>());
                subscriptions.Add(RealmDatabase.All<TextEntity>());
                subscriptions.Add(RealmDatabase.All<PhraseEntity>());
                subscriptions.Add(RealmDatabase.All<SourceEntity>());
                subscriptions.Add(RealmDatabase.All<LanguageEntity>());
                subscriptions.Add(RealmDatabase.All<UserEntity>());
            });
        }
        private async Task<Realm> GetSyncedRealmInstance()
        {
            var config = new FlexibleSyncConfiguration(_user);
            return await Realm.GetInstanceAsync(config);
        }
        private async Task ConnectToSyncedDatabase()
        {
            var myRealmAppId = "dosham-ahtcj";
            _app = App.Create(myRealmAppId);
            _user = await _app.LogInAsync(Credentials.Anonymous());

            var config = new FlexibleSyncConfiguration(_user);
            RealmDatabase = await Realm.GetInstanceAsync(config);

            AddSubscriptionsToRealm();
        }

        private async void CopyFromLocalToSyncedRealm()
        {
            RealmDatabase = await GetRealmInstanceAsync();
            _syncedRealm.Write(() =>
            {
                var languages = RealmDatabase.All<LanguageEntity>();
                foreach (var language in languages)
                {
                    _syncedRealm.Add(new LanguageEntity() { Code = language.Code, _id = language._id });
                }

                var sources = RealmDatabase.All<SourceEntity>();
                foreach (var item in sources)
                {
                    _syncedRealm.Add(new SourceEntity()
                    {
                        _id = item._id,
                        UpdatedAt = item.UpdatedAt,
                        Name = item.Name,
                        Notes = item.Notes,
                    });
                }

                var users = RealmDatabase.All<UserEntity>();
                foreach (var item in users)
                {
                    _syncedRealm.Add(new UserEntity()
                    {
                        _id = item._id,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        Password = item.Password,
                        Username = item.Username,
                        UpdatedAt = item.UpdatedAt,
                    });
                }

                var entries = RealmDatabase.All<EntryEntity>();
                foreach (var entry in entries)
                {
                    var newEntry = new EntryEntity()
                    {
                        _id = entry._id,
                        UpdatedAt = entry.UpdatedAt,
                        Rate = entry.Rate,
                        RawContents = entry.RawContents,
                        Source = _syncedRealm.All<SourceEntity>().FirstOrDefault(s => s.Name == entry.Source.Name),
                        Type = entry.Type,
                        User = _syncedRealm.All<UserEntity>().FirstOrDefault(u => u.Username == entry.User.Username),
                    };

                    foreach (var translation in entry.Translations)
                    {
                        var newTranslation = new TranslationEntity()
                        {
                            _id = translation._id,
                            UpdatedAt = translation.UpdatedAt,
                            Content = translation.Content,
                            Notes = translation.Notes,
                            Rate = translation.Rate,
                            RawContents = translation.RawContents,
                            Entry = newEntry,
                            Language = _syncedRealm.All<LanguageEntity>().FirstOrDefault(l => l.Code == translation.Language.Code),
                            User = _syncedRealm.All<UserEntity>().FirstOrDefault(u => u.Username == translation.User.Username),
                        };

                        newEntry.Translations.Add(newTranslation);
                    }

                    switch (entry.Type)
                    {
                        case EntryType.Word:
                            var word = new WordEntity()
                            {
                                _id = entry.Word._id,
                                UpdatedAt = entry.Word.UpdatedAt,
                                Entry = newEntry,
                                Content = entry.Word.Content,
                                Forms = entry.Word.Forms,
                                GrammaticalClass = entry.Word.GrammaticalClass,
                                Notes = entry.Word.Notes,
                                NounDeclensions = entry.Word.NounDeclensions,
                                VerbTenses = entry.Word.VerbTenses,
                                PartOfSpeech = entry.Word.PartOfSpeech,
                            };

                            newEntry.Word = word;
                            break;

                        case EntryType.Phrase:
                            var phrase = new PhraseEntity()
                            {
                                _id = entry.Phrase._id,
                                UpdatedAt = entry.Phrase.UpdatedAt,
                                Entry = newEntry,
                                Content = entry.Phrase.Content,
                                Notes = entry.Phrase.Content
                            };

                            newEntry.Phrase = phrase;
                            break;

                        default:
                            break;
                    }

                    _syncedRealm.Add(newEntry);
                }

            });
        }

        public  void DoDangerousTheStuff()
        {
            var entries = RealmDatabase.All<EntryEntity>().ToList();
            var s = RealmDatabase.Subscriptions.State;
            //await RealmDatabase.Subscriptions.WaitForSynchronizationAsync();
            var g = 22;

            //CopyObjectIds();
            //RemoveWeirdos();
            //SetSourceNotes();
            //ImportPhrases();
            //UpdateEntryRawContentField();
            //RemoveDuplicates();
            //SetTranslationEntryAndUserLinks();
            //RemoveExistingDuplicatingInLegacyPhrases();

        }
        private static RealmConfiguration GetLocalRealmConfiguration()
        {
            var dbPath = Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName);

            return new RealmConfiguration(dbPath)
            {
                SchemaVersion = 14,
                ShouldCompactOnLaunch = (totalBytes, usedBytes) =>
                {
                    ulong oneHundredMB = 30 * 1024 * 1024;
                    return totalBytes > oneHundredMB && usedBytes / totalBytes < 0.5;
                }
            };
        }
        public RealmDataAccessService()
        {

            var fs = new FileService();
            fs.PrepareDatabase();

            Task.Run(async () =>
            {
                await ConnectToSyncedDatabase();
                DoDangerousTheStuff();
            }).Wait();
        }

        public Action<string, SearchResultsModel> NewSearchResults;
        private App _app;
        private User _user;

        public async Task<IEnumerable<EntryModel>> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = (await GetRealmInstanceAsync()).All<EntryEntity>().AsEnumerable();

            // Takes random entries and shuffles them to break the natural order
            return entries
                .Where(entry => entry.Rate > 0)
                .OrderBy(x => randomizer.Next(0, 70000))
                .Take(RandomEntriesLimit)
                .OrderBy(entry => entry.GetHashCode())
                .Select(entry => new EntryModel(entry));
        }

        public async Task FindAsync(string inputText)
        {
            var searchEngine = new MainSearchEngine(this);
            await searchEngine.FindAsync(inputText);
        }

        internal async Task<Realm> GetRealmInstanceAsync()
        {
            return await GetSyncedRealmInstance();
            //Realm.GetInstance(GetRealmConfiguration());
        }
    }
}
