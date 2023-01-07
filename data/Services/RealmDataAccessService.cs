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

namespace Data.Services
{
    public class RealmDataAccessService : IDataAccessService
    {

        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;
        
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

            var distinctEntryIds = entries.AsEnumerable().DistinctBy(e => e.RawContents).Select(e => e.Id).ToArray();

            var duplicatingEntryIds = new List<ObjectId>();
            foreach (var entry in entries)
            {
                if (!distinctEntryIds.Contains(entry.Id))
                {
                    duplicatingEntryIds.Add(entry.Id);
                }
            }

            var entriesToRemove = new HashSet<EntryEntity>();
            foreach (var entry in entries.AsEnumerable().Where(e => duplicatingEntryIds.Contains(e.Id)))
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

        public void DoDangerousTheStuff()
        {
            //SetSourceNotes();
            //ImportPhrases();
            //UpdateEntryRawContentField();
            //RemoveDuplicates();
            //SetTranslationEntryAndUserLinks();
            //RemoveExistingDuplicatingInLegacyPhrases();
        }
        private static RealmConfiguration GetRealmConfiguration()
        {
            var dbPath = Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName);

            return new RealmConfiguration(dbPath)
            {
                SchemaVersion = 5,
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
         
            RealmDatabase = Realm.GetInstance(GetRealmConfiguration());
        }

        public Action<string, SearchResultsModel> NewSearchResults;

        public IEnumerable<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = GetRealmInstance().All<EntryEntity>().AsEnumerable();

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

        internal Realm GetRealmInstance()
        {
            return Realm.GetInstance(GetRealmConfiguration());
        }
    }
}
