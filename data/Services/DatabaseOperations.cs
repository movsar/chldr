﻿using Data.Entities;
using Data.Enums;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    internal class DatabaseOperations
    {
        Realm _localRealm, _syncedRealm;

        private async Task CopyEntriesByChunks(IEnumerable<EntryEntity> entries)
        {
            await _syncedRealm.WriteAsync(() =>
            {
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

        private async Task CopyFromLocalToSyncedRealm()
        {
            //RealmDatabase = GetRealmInstance();
            _syncedRealm.Write(() =>
            {
                var languages = _localRealm.All<LanguageEntity>();
                foreach (var language in languages)
                {
                    _syncedRealm.Add(new LanguageEntity() { Code = language.Code, _id = language._id });
                }

                var sources = _localRealm.All<SourceEntity>();
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

                var users = _localRealm.All<UserEntity>();
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
            });

            await _syncedRealm.Subscriptions.WaitForSynchronizationAsync();

            Debug.WriteLine($"Starting inserting entries");
            var entries = _localRealm.All<EntryEntity>().AsEnumerable().OrderBy(e => e._id);
            int totalCount = entries.Count();
            for (int i = 0; i < totalCount; i = i + 200)
            {
                var entriesToUpload = entries.Skip(i);
                entriesToUpload = entriesToUpload.Count() > 200 ? entriesToUpload.Take(200) : entriesToUpload;
                await CopyEntriesByChunks(entriesToUpload);

                //_syncedRealm = await GetSyncedRealmInstance();
                await _syncedRealm.Subscriptions.WaitForSynchronizationAsync();

                Debug.WriteLine($"Entries inserted: {i}");
            }
        }

        private async Task UpdatePhrasesByChunks()
        {
            Debug.WriteLine($"Starting inserting entries");
            var localPhrases = _localRealm.All<PhraseEntity>().AsEnumerable();
            int totalCount = localPhrases.Count();

            for (int i = 0; i < totalCount; i = i + 200)
            {
                var itemsToUpdate = localPhrases.Skip(i);
                itemsToUpdate = itemsToUpdate.Count() > 200 ? itemsToUpdate.Take(200) : itemsToUpdate;

                await _localRealm.WriteAsync(() =>
                {
                    foreach (var localPhrase in itemsToUpdate)
                    {
                        _localRealm.All<PhraseEntity>().First(p => p._id == localPhrase._id).Notes = localPhrase.Notes;
                    }
                });

                await _localRealm.Subscriptions.WaitForSynchronizationAsync();

                Debug.WriteLine($"Phrases updated: {i}");
            }

        }

        private void SetRatesForOfficialDictionaryEntries()
        {
            _localRealm.Write(() =>
            {
                Debug.WriteLine("Doing stuff");

                foreach (var entry in _localRealm.All<EntryEntity>().AsEnumerable().Where(entry =>
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
            _localRealm.Write(() =>
            {
                Debug.WriteLine("Doing stuff");

                foreach (var entry in _localRealm.All<EntryEntity>())
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

                foreach (var translation in _localRealm.All<TranslationEntity>())
                {
                    translation.RawContents = translation.Content.ToLower();
                }
            });
        }
        private void UpdateFormsField()
        {
            // This must be done either automatically or manually from time to time
            _localRealm.Write(() =>
            {
                Debug.WriteLine("Doing stuff");
                foreach (var entry in _localRealm.All<EntryEntity>())
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
            _localRealm.Write(() =>
            {
                Debug.WriteLine("Doing stuff");
                foreach (var entry in _localRealm.All<EntryEntity>())
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
            _localRealm.Write(() =>
            {
                List<SourceEntity> sources = new List<SourceEntity>
                {
                    new SourceEntity(){ Name = "GEO"},
                    new SourceEntity(){ Name = "Yurslovar"},
                };

                foreach (var s in sources)
                {
                    _localRealm.Add(s);
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
            var adminUser = _localRealm.All<UserEntity>().First();

            _localRealm.Write(() =>
            {
                foreach (var entry in _localRealm.All<EntryEntity>())
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
            var translations = _localRealm.All<TranslationEntity>();
            //translations.AsEnumerable().Select(translation => new { Translation = translation.Content, LanguageCode = translation.Language.Code });
            var entries = _localRealm.All<EntryEntity>();

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

            _localRealm.Write(() =>
            {
                foreach (var entry in entriesToRemove)
                {
                    switch (entry.Type)
                    {
                        case EntryType.Word:
                            _localRealm.Remove(entry.Word);
                            break;
                        case EntryType.Phrase:
                            _localRealm.Remove(entry.Phrase);
                            break;
                        case EntryType.Text:
                            _localRealm.Remove(entry.Text);
                            break;
                    }

                    foreach (var translation in entry.Translations)
                    {
                        _localRealm.Remove(translation);
                    }

                    _localRealm.Remove(entry);
                    counter++;
                }
            });

            Debug.WriteLine($"Removed {counter} duplicates");
        }
        private void SetSourceNotes()
        {
            _localRealm.Write(() =>
            {
                var source = _localRealm.All<SourceEntity>().First(s => s.Name == "1");
                source.Name = "ikhasakhanov";
                source.Notes = "";
            });
        }
        private void RemoveWeirdos()
        {
            _localRealm.Write(() =>
            {
                var thing = "Ψ".ToLower();
                var phrases = _localRealm.All<PhraseEntity>().Where(p => p.Content.Contains("Ψ") || p.Notes.Contains("Ψ")).ToList();
                var words = _localRealm.All<WordEntity>().Where(w => w.Notes.Contains("Ψ")).ToList();

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
            _localRealm.Write(() =>
            {
                var entries = _localRealm.All<EntryEntity>();
                foreach (var entity in entries)
                {
                    entity._id = entity._id;
                }

                var translations = _localRealm.All<TranslationEntity>();
                foreach (var entity in translations)
                {
                    entity._id = entity._id;
                }

                var languages = _localRealm.All<LanguageEntity>();
                foreach (var entity in languages)
                {
                    entity._id = entity._id;
                }

                var phrases = _localRealm.All<PhraseEntity>();
                foreach (var entity in phrases)
                {
                    entity._id = entity._id;
                }

                var users = _localRealm.All<UserEntity>();
                foreach (var entity in users)
                {
                    entity._id = entity._id;
                }

                var words = _localRealm.All<WordEntity>();
                foreach (var entity in words)
                {
                    entity._id = entity._id;
                }

                var sources = _localRealm.All<SourceEntity>();
                foreach (var entity in sources)
                {
                    entity._id = entity._id;
                }
            });
        }

    }
}