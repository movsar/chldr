using chldr_data.Entities;
using chldr_data.Enums;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Realms;
using Realms.Sync;
using Entities = chldr_data.Entities;

namespace chldr_tools.Services
{
    public class DatabaseOperations
    {
        //static Realm _localRealm, _syncedRealm;
        //private IRealmServiceFactory _realmServiceFactory;
        //private static FileService _fileService;

        //private void EncryptDatbase(string path)
        //{
        //    //using var realm = _realmServiceFactory;

        //    //var encryptionKey = new byte[64];
        //    //using var rng = new RNGCryptoServiceProvider();
        //    //rng.GetBytes(encryptionKey);

        //    //var encryptedConfig = new RealmConfiguration(path)
        //    //{
        //    //    EncryptionKey = encryptionKey
        //    //};

        //    //realm.WriteCopy(encryptedConfig);

        //    //// Save key
        //    //File.WriteAllBytes("encryption.key", encryptionKey);
        //}
        //private async Task RunSyncedDatabaseMaintenanceAsync()
        //{
        //    var realmService = _realmServiceFactory.GetInstance(DataSourceType.Synced) as SyncedRealmService;
        //    var app = realmService.GetApp();
        //    //await app.LogInAsync(Credentials.Anonymous());
        //    realmService.InitializeConfiguration();
        //    if (realmService.GetDatabase() == null)
        //    {
        //        throw new Exception("Database is null");
        //    }

        //    _syncedRealm = realmService.GetDatabase();
        //    _localRealm = GetLocalRealm();

        //    await CopyFromLocalToSyncedRealm(500);
        //}
        //public async Task RunMaintenanceAsync(IRealmServiceFactory realmServiceFactory)
        //{
        //    _realmServiceFactory = realmServiceFactory;
        //    _fileService = new FileService(AppContext.BaseDirectory);

        //    //await RunSyncedDatabaseMaintenanceAsync();
        //}
        //private static Realm GetLocalRealm()
        //{
        //    byte[] encKey = AppConstants.EncKey.Split(":").Select(numAsString => Convert.ToByte(numAsString)).ToArray();

        //    var encryptedConfig = new RealmConfiguration(_fileService.OfflineDatabaseFilePath)
        //    {
        //        SchemaVersion = 1,
        //        EncryptionKey = encKey
        //    };

        //    return Realm.GetInstance(encryptedConfig);
        //}
        //public void CopySqlToRealm()
        //{
        //    var sqlContext = new SqlContext();
        //    var realmContext = Realm.GetInstance();

        //    realmContext.Write((Action)(() =>
        //{
        //    var users = sqlContext.Users;
        //    foreach (var item in users)
        //    {
        //        realmContext.Add<RealmUser>(new RealmUser()
        //        {
        //            UserId = item.UserId,
        //            AccountStatus = item.AccountStatus,
        //            ImagePath = item.ImagePath,
        //            IsModerator = item.IsModerator,
        //            LastName = item.LastName,
        //            Password = item.Password,
        //            Patronymic = item.Patronymic,
        //            Rate = item.Rate,
        //            Email = item.Email,
        //            FirstName = item.FirstName,
        //        });
        //    }

        //    var languages = sqlContext.Languages;
        //    foreach (var language in languages)
        //    {
        //        realmContext.Add<RealmLanguage>(new RealmLanguage()
        //        {
        //            LanguageId = language.LanguageId,
        //            Code = language.Code,
        //            CreatedAt = language.CreatedAt,
        //            Name = language.Name,
        //            UpdatedAt = language.UpdatedAt,
        //            User = realmContext.Find<RealmUser>(language.UserId)
        //        });
        //    }

        //    var sources = sqlContext.Sources;
        //    foreach (var item in sources)
        //    {
        //        realmContext.Add<RealmSource>(new RealmSource()
        //        {
        //            SourceId = item.SourceId,
        //            CreatedAt = item.CreatedAt,
        //            UpdatedAt = item.UpdatedAt,
        //            User = realmContext.Find<RealmUser>(item.UserId),
        //            Name = item.Name,
        //            Notes = item.Notes,
        //        });
        //    }



        //    foreach (var entry in sqlContext.Entries
        //        .Include<SqlEntry, SqlWord>(e => e.Word)
        //        .Include<SqlEntry, SqlPhrase>(e => e.Phrase)
        //        .Include<SqlEntry, SqlText>(e => e.Text)
        //        .Include<SqlEntry, ICollection<SqlTranslation>>(e => e.Translations))
        //    {
        //        var newEntry = new Entities.RealmEntry()
        //        {
        //            EntryId = entry.EntryId,
        //            Source = realmContext.Find<RealmSource>(entry.SourceId),
        //            User = realmContext.Find<RealmUser>(entry.UserId),
        //            Rate = entry.Rate,
        //            RawContents = entry.RawContents,
        //            Type = entry.Type,
        //            CreatedAt = entry.CreatedAt,
        //            UpdatedAt = entry.UpdatedAt,
        //        };
        //        realmContext.Add<RealmEntry>((RealmEntry)newEntry);

        //        foreach (var translation in entry.Translations)
        //        {
        //            var newTranslation = new RealmTranslation()
        //            {
        //                TranslationId = translation.TranslationId,
        //                Entry = realmContext.Find<Entities.RealmEntry>(translation.EntryId),
        //                Language = realmContext.Find<RealmLanguage>(translation.LanguageId),
        //                User = realmContext.Find<RealmUser>(translation.UserId),
        //                Content = translation.Content,
        //                RawContents = translation.RawContents,
        //                Notes = translation.Notes,
        //                Rate = translation.Rate,
        //                CreatedAt = entry.CreatedAt,
        //                UpdatedAt = entry.UpdatedAt,
        //            };

        //            realmContext.Add<RealmTranslation>(newTranslation);
        //            newEntry.Translations.Add(newTranslation);
        //        }

        //        switch ((EntryType)entry.Type)
        //        {
        //            case EntryType.Word:
        //                var word = new RealmWord()
        //                {
        //                    WordId = entry.Word.WordId,
        //                    Entry = realmContext.Find<Entities.RealmEntry>(entry.Word.EntryId),
        //                    Content = entry.Word.Content,
        //                    Notes = entry.Word.Notes,
        //                    PartOfSpeech = entry.Word.PartOfSpeech,
        //                };

        //                realmContext.Add<RealmWord>(word);
        //                newEntry.Word = word;
        //                break;

        //            case EntryType.Phrase:
        //                var phrase = new RealmPhrase()
        //                {
        //                    PhraseId = entry.Phrase.PhraseId,
        //                    Entry = realmContext.Find<Entities.RealmEntry>(entry.Phrase.EntryId),
        //                    Content = entry.Phrase.Content,
        //                    Notes = entry.Phrase.Notes
        //                };

        //                realmContext.Add<RealmPhrase>(phrase);
        //                newEntry.Phrase = phrase;
        //                break;

        //            default:
        //                break;
        //        }

        //    }
        //}));
        //}

        //private async Task CopyFromLocalToSyncedRealm(int limit = 9999999)
        //{
        //    _syncedRealm.Write(() =>
        //    {
        //        var languages = _localRealm.All<Language>();
        //        foreach (var language in languages)
        //        {
        //            _syncedRealm.Add(new Language() { Code = language.Code, _id = language._id });
        //        }

        //        var sources = _localRealm.All<Source>();
        //        foreach (var item in sources)
        //        {
        //            _syncedRealm.Add(new Source()
        //            {
        //                _id = item._id,
        //                UpdatedAt = item.UpdatedAt,
        //                Name = item.Name,
        //                Notes = item.Notes,
        //            });
        //        }

        //        var users = _localRealm.All<User>();
        //        foreach (var item in users)
        //        {
        //            _syncedRealm.Add(new User()
        //            {
        //                _id = item._id,
        //                Email = item.Email,
        //                FirstName = item.FirstName,
        //                UpdatedAt = item.UpdatedAt,
        //            });
        //        }
        //    });

        //    await _syncedRealm.Subscriptions.WaitForSynchronizationAsync();

        //    Debug.WriteLine($"Starting inserting entries");
        //    var entries = _localRealm.All<Entry>().Where(e => e.Type == (int)EntryType.Phrase).AsEnumerable().OrderBy(e => e._id);
        //    int totalCount = entries.Count();
        //    for (int i = 0; i <= totalCount; i = i + 200)
        //    {
        //        var entriesToUpload = entries.Skip(i);
        //        entriesToUpload = entriesToUpload.Count() > 200 ? entriesToUpload.Take(200) : entriesToUpload;

        //        CopyEntriesByChunks(entriesToUpload);

        //        //_syncedRealm = await GetSyncedRealmInstance();
        //        await _syncedRealm.Subscriptions.WaitForSynchronizationAsync();


        //        Debug.WriteLine($"Entries inserted: {i}");

        //        if (i == limit)
        //        {
        //            break;
        //        }
        //    }
        //}

        //private void UpdatePhrasesByChunks()
        //{
        //    _localRealm = GetLocalRealm();

        //    Debug.WriteLine($"Starting updating entries");
        //    var localPhrases = _localRealm.All<Phrase>().AsEnumerable();
        //    int totalCount = localPhrases.Count();

        //    _syncedRealm.Write(() =>
        //    {
        //        for (int i = 0; i < totalCount; i = i + 200)
        //        {
        //            var itemsToUpdate = localPhrases.Skip(i);
        //            itemsToUpdate = itemsToUpdate.Count() > 200 ? itemsToUpdate.Take(200) : itemsToUpdate;


        //            foreach (var localPhrase in itemsToUpdate)
        //            {
        //                _syncedRealm.All<Phrase>().First(p => p._id == localPhrase._id).Notes = localPhrase.Notes;
        //            }

        //            Debug.WriteLine($"Phrase updated");
        //        }
        //    });

        //    Debug.WriteLine($"All phrases updated");

        //}

        //private void SetRatesForOfficialDictionaryEntries()
        //{
        //    _localRealm.Write(() =>
        //    {
        //        Debug.WriteLine("Doing stuff");

        //        foreach (var entry in _localRealm.All<Entry>().AsEnumerable().Where(entry =>
        //            entry.Source.Name == "Maciev"
        //            || entry.Source.Name == "Anatslovar"
        //            || entry.Source.Name == "Malaev"
        //            || entry.Source.Name == "Karasaev"
        //        ))
        //        {
        //            entry.Rate = 5;
        //            foreach (var translation in entry.Translations)
        //            {
        //                translation.Rate = 5;
        //            }
        //        }
        //    });
        //}
        //private void UpdateEntryRawContentField()
        //{
        //    // This must be done either automatically or manually from time to time to improve search speed
        //    _localRealm.Write(() =>
        //    {
        //        Debug.WriteLine("Doing stuff");

        //        foreach (var entry in _localRealm.All<Entities.Entry>())
        //        {
        //            switch ((EntryType)entry.Type)
        //            {
        //                case EntryType.Word:
        //                    entry.RawContents = entry.Word.Content.ToLower();
        //                    break;

        //                case EntryType.Phrase:
        //                    entry.RawContents = entry.Phrase.Content.ToLower();
        //                    break;
        //            }
        //        }

        //        foreach (var translation in _localRealm.All<Translation>())
        //        {
        //            translation.RawContents = translation.Content.ToLower();
        //        }
        //    });
        //}
        //private void UpdateFormsField()
        //{
        //    // This must be done either automatically or manually from time to time
        //    _localRealm.Write(() =>
        //    {
        //        Debug.WriteLine("Doing stuff");
        //        foreach (var entry in _localRealm.All<Entities.Entry>())
        //        {
        //            if (entry.Type != (int)EntryType.Word)
        //            {
        //                continue;
        //            }

        //        }

        //    });
        //}
        //private void DeclensionsAndTensesToLower()
        //{
        //    _localRealm.Write(() =>
        //    {
        //        Debug.WriteLine("Doing stuff");
        //        foreach (var entry in _localRealm.All<Entities.Entry>())
        //        {
        //            if ((EntryType)entry.Type != EntryType.Word)
        //            {
        //                continue;
        //            }
        //        }
        //    });
        //}
        //private void AddSources()
        //{
        //    _localRealm.Write(() =>
        //    {
        //        List<Source> sources = new List<Source>
        //        {
        //            new Source(){ Name = "GEO"},
        //            new Source(){ Name = "Yurslovar"},
        //        };

        //        foreach (var s in sources)
        //        {
        //            _localRealm.Add(s);
        //        }

        //    });
        //}
        //private void ImportPhrases()
        //{
        //    //var legacyPhrases = LegacyEntriesProvider.GetLegacyPhraseEntries();
        //    //var legacyTranslations = LegacyEntriesProvider.GetLegacyPhraseTranslationEntries();

        //    //var yurslovarSource = RealmDatabase.All<SourceEntity>().Where(source => source.Name.Equals("Yurslovar")).First();
        //    //var geoSource = RealmDatabase.All<SourceEntity>().Where(source => source.Name.Equals("GEO")).First();
        //    //var userSource = RealmDatabase.All<SourceEntity>().Where(source => source.Name.Equals("User")).First();

        //    //var adminUser = RealmDatabase.All<UserEntity>().First();

        //    //var langs = legacyTranslations.DistinctBy(t => t.LanguageCode);

        //    //int counter = 0;
        //    //RealmDatabase.Write(() =>
        //    //{
        //    //    foreach (var phraseEntry in legacyPhrases)
        //    //    {
        //    //        // Find or insert the source
        //    //        SourceEntity source = null;
        //    //        if (string.IsNullOrWhiteSpace(phraseEntry.Source))
        //    //        {
        //    //            source = RealmDatabase.All<SourceEntity>().FirstOrDefault(source => source.Name.Equals(phraseEntry.UserId.ToString()));
        //    //            if (source == null)
        //    //            {
        //    //                source = new SourceEntity() { Name = phraseEntry.UserId.ToString(), Notes = "Imported from legacy database" };
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            source = RealmDatabase.All<SourceEntity>().FirstOrDefault(source => source.Name.Equals(phraseEntry.Source, StringComparison.OrdinalIgnoreCase));
        //    //            if (source == null)
        //    //            {
        //    //                source = new SourceEntity() { Name = phraseEntry.Source, Notes = "Imported from legacy database" };
        //    //            }
        //    //        }

        //    //        // Initialize an entry object
        //    //        var entry = new EntryEntity()
        //    //        {
        //    //            User = adminUser,
        //    //            Rate = Convert.ToInt32(phraseEntry.Rate),
        //    //            Source = source,
        //    //        };

        //    //        // Insert data
        //    //        switch (phraseEntry.Type.ToLower())
        //    //        {
        //    //            case "word":
        //    //                var word = new WordEntity()
        //    //                {
        //    //                    Entry = entry,
        //    //                    Content = phraseEntry.Phrase,
        //    //                    Forms = phraseEntry.Forms,
        //    //                    Notes = phraseEntry.Notes
        //    //                };

        //    //                RealmDatabase.Add(word);

        //    //                entry.Type = EntryType.Word;
        //    //                entry.Word = word;

        //    //                break;
        //    //            case "phrase":
        //    //                var phrase = new PhraseEntity()
        //    //                {
        //    //                    Entry = entry,
        //    //                    Content = phraseEntry.Phrase,
        //    //                    Notes = phraseEntry.Notes
        //    //                };

        //    //                RealmDatabase.Add(phrase);

        //    //                entry.Type = EntryType.Phrase;
        //    //                entry.Phrase = phrase;
        //    //                break;
        //    //            default:
        //    //                break;
        //    //        }

        //    //        RealmDatabase.Add(entry);

        //    //        // Insert translations
        //    //        foreach (var t in legacyTranslations.Where(t => t.PhraseId == phraseEntry.Id))
        //    //        {
        //    //            var translation = new TranslationEntity()
        //    //            {
        //    //                Content = t.Translation,
        //    //                Language = RealmDatabase.All<LanguageEntity>().First(l => l.Code == t.LanguageCode),
        //    //                Entry = entry,
        //    //                Rate = Convert.ToInt32(t.Rate),
        //    //                User = adminUser,
        //    //            };

        //    //            RealmDatabase.Add(translation);
        //    //            entry.Translations.Add(translation);
        //    //        }

        //    //        counter++;
        //    //    }
        //    //});

        //    //Debug.WriteLine($"Added {counter} entries");
        //}
        //private void SetTranslationEntryAndUserLinks()
        //{
        //    var adminUser = _localRealm.All<User>().First();

        //    _localRealm.Write(() =>
        //    {
        //        foreach (var entry in _localRealm.All<Entities.Entry>())
        //        {
        //            entry.User = adminUser;

        //            foreach (var translation in entry.Translations)
        //            {
        //                translation.User = adminUser;
        //                translation.Entry = entry;
        //            }
        //        }
        //    });
        //}
        //private void RemoveDuplicates()
        //{
        //    int counter = 0;
        //    var translations = _localRealm.All<Translation>();
        //    //translations.AsEnumerable().Select(translation => new { Translation = translation.Content, LanguageCode = translation.Language.Code });
        //    var entries = _localRealm.All<Entities.Entry>();

        //    var distinctEntryIds = entries.AsEnumerable().DistinctBy(e => e.RawContents).Select(e => e._id).ToArray();

        //    var duplicatingEntryIds = new List<ObjectId>();
        //    foreach (var entry in entries)
        //    {
        //        if (!distinctEntryIds.Contains(entry._id))
        //        {
        //            duplicatingEntryIds.Add(entry._id);
        //        }
        //    }

        //    var entriesToRemove = new HashSet<Entities.Entry>();
        //    foreach (var entry in entries.AsEnumerable().Where(e => duplicatingEntryIds.Contains(e._id)))
        //    {
        //        foreach (var translation in entry.Translations)
        //        {
        //            if (translations.Count(t => t.Content == translation.Content && t.Language == translation.Language) > 1)
        //            {
        //                entriesToRemove.Add(translation.Entry);
        //            }
        //        }
        //    }

        //    _localRealm.Write(() =>
        //    {
        //        foreach (var entry in entriesToRemove)
        //        {
        //            switch ((EntryType)entry.Type)
        //            {
        //                case EntryType.Word:
        //                    _localRealm.Remove(entry.Word);
        //                    break;
        //                case EntryType.Phrase:
        //                    _localRealm.Remove(entry.Phrase);
        //                    break;
        //                case EntryType.Text:
        //                    _localRealm.Remove(entry.Text);
        //                    break;
        //            }

        //            foreach (var translation in entry.Translations)
        //            {
        //                _localRealm.Remove(translation);
        //            }

        //            _localRealm.Remove(entry);
        //            counter++;
        //        }
        //    });

        //    Debug.WriteLine($"Removed {counter} duplicates");
        //}
        //private void SetSourceNotes()
        //{
        //    _localRealm.Write(() =>
        //    {
        //        var source = _localRealm.All<Source>().First(s => s.Name == "1");
        //        source.Name = "ikhasakhanov";
        //        source.Notes = "";
        //    });
        //}
        //private void RemoveWeirdos()
        //{
        //    _localRealm.Write(() =>
        //    {
        //        var thing = "Ψ".ToLower();
        //        var phrases = _localRealm.All<Phrase>().Where(p => p.Content.Contains("Ψ") || p.Notes.Contains("Ψ")).ToList();
        //        var words = _localRealm.All<Word>().Where(w => w.Notes.Contains("Ψ")).ToList();

        //        foreach (var phrase in phrases)
        //        {
        //            if (phrase.Notes.Length < 6)
        //            {
        //                phrase.Notes = null;
        //                continue;
        //            }

        //            var translationNotes = phrase.Notes.Split("Ψ");
        //            foreach (var translationNote in translationNotes)
        //            {
        //                if (string.IsNullOrWhiteSpace(translationNote))
        //                {
        //                    phrase.Notes = null;
        //                    continue;
        //                }
        //                var parts = translationNote.Split(":");
        //                var languageCode = parts[0];
        //                var note = parts[1];

        //                var translation = phrase.Entry.Translations.FirstOrDefault(t => t.Language.Code == languageCode);
        //                if (translation == null)
        //                {
        //                    phrase.Notes = note;
        //                }
        //                else
        //                {
        //                    translation.Notes = note;
        //                    phrase.Notes = null;
        //                }
        //            }
        //        }

        //        foreach (var word in words)
        //        {
        //            if (word.Notes.Length < 6)
        //            {
        //                word.Notes = null;
        //                continue;
        //            }
        //            var translationNotes = word.Notes.Split("Ψ");
        //            foreach (var translationNote in translationNotes)
        //            {
        //                if (string.IsNullOrWhiteSpace(translationNote))
        //                {
        //                    word.Notes = null;
        //                    continue;
        //                }
        //                var parts = translationNote.Split(":");
        //                var languageCode = parts[0];
        //                var note = parts[1];

        //                var translation = word.Entry.Translations.FirstOrDefault(t => t.Language.Code == languageCode);
        //                if (translation == null)
        //                {
        //                    word.Notes = note;
        //                }
        //                else
        //                {
        //                    translation.Notes = note;
        //                    word.Notes = null;
        //                }
        //            }
        //        }
        //    });
        //}
        //private void CopyObjectIds()
        //{
        //    _localRealm.Write(() =>
        //    {
        //        var entries = _localRealm.All<Entities.Entry>();
        //        foreach (var entity in entries)
        //        {
        //            entity._id = entity._id;
        //        }

        //        var translations = _localRealm.All<Translation>();
        //        foreach (var entity in translations)
        //        {
        //            entity._id = entity._id;
        //        }

        //        var languages = _localRealm.All<Language>();
        //        foreach (var entity in languages)
        //        {
        //            entity._id = entity._id;
        //        }

        //        var phrases = _localRealm.All<Phrase>();
        //        foreach (var entity in phrases)
        //        {
        //            entity._id = entity._id;
        //        }

        //        var users = _localRealm.All<User>();
        //        foreach (var entity in users)
        //        {
        //            entity._id = entity._id;
        //        }

        //        var words = _localRealm.All<Word>();
        //        foreach (var entity in words)
        //        {
        //            entity._id = entity._id;
        //        }

        //        var sources = _localRealm.All<Source>();
        //        foreach (var entity in sources)
        //        {
        //            entity._id = entity._id;
        //        }
        //    });
        //}

    }
}
