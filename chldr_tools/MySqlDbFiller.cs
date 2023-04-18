using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Services;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_tools
{
    public class MySqlDbFiller
    {
        private readonly OfflineRealmService _realmService;
        private readonly Realm _realm;
        public MySqlDbFiller(OfflineRealmService realmService)
        {
            _realmService = realmService;
            _realm = realmService.GetDatabase();
        }
        internal void Run()
        {
            FillEntries();
        }

        internal void FillLanguages()
        {
            var languages = _realm.All<Language>();
            foreach (var language in languages)
            {

            }
        }
        internal void FillEntries()
        {
            foreach (var entry in _realm.All<Entry>())
            {
                var newEntry = new chldr_data.Entities.Entry()
                {
                    _id = entry._id,
                    UpdatedAt = entry.UpdatedAt,
                    Rate = entry.Rate,
                    RawContents = entry.RawContents,
                    Source = _realm.All<Source>().FirstOrDefault(s => s.Name == entry.Source.Name),
                    Type = entry.Type,
                    User = _realm.All<User>().FirstOrDefault(u => u.Email == entry.User.Email),
                };

                foreach (var translation in entry.Translations)
                {
                    var newTranslation = new Translation()
                    {
                        _id = translation._id,
                        UpdatedAt = translation.UpdatedAt,
                        Content = translation.Content,
                        Notes = translation.Notes,
                        Rate = translation.Rate,
                        RawContents = translation.RawContents,
                        Entry = newEntry,
                        Language = _realm.All<Language>().FirstOrDefault(l => l.Code == translation.Language.Code),
                        User = _realm.All<User>().FirstOrDefault(u => u.Email == translation.User.Email),
                    };

                    newEntry.Translations.Add(newTranslation);
                }

                switch ((EntryType)entry.Type)
                {
                    case EntryType.Word:
                        var word = new Word()
                        {
                            _id = entry.Word._id,
                            UpdatedAt = entry.Word.UpdatedAt,
                            Entry = newEntry,
                            Content = entry.Word.Content,
                            //GrammaticalClass = entry.Word.GrammaticalClass,
                            Notes = entry.Word.Notes,
                            PartOfSpeech = entry.Word.PartOfSpeech,
                        };

                        newEntry.Word = word;
                        break;

                    case EntryType.Phrase:
                        var phrase = new Phrase()
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

                // newEntry
            }
        }
    }
}
