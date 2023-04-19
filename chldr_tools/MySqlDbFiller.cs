using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Services;
using chldr_data.Entities;
using Realms;
using User = chldr_data.Entities.User;

namespace chldr_tools
{
    public class MySqlDbFiller
    {
        private readonly SqlContext _context;
        private readonly OfflineRealmService _realmService;
        private readonly Realm _realm;
        public MySqlDbFiller(OfflineRealmService realmService)
        {
            _context = new SqlContext();
            _realmService = realmService;
            _realm = realmService.GetDatabase();
        }
        internal void Run()
        {
            InsertUsers();
            InsertLanguages();
            InsertSources();
            InsertEntries();
            InsertTranslations();
        }

        private void InsertTranslations()
        {
            foreach (var translation in _realm.All<Translation>())
            {
                var sqlTranslation = new Translation()
                {
                    //TranslationId = translation._id.ToString(),
                    //EntryId = translation.Entry._id.ToString(),
                    //UserId = translation.User._id.ToString(),
                    //LanguageId = translation.Language._id.ToString(),
                    CreatedAt = translation.CreatedAt.UtcDateTime,
                    UpdatedAt = translation.UpdatedAt.UtcDateTime,
                    Rate = translation.Rate,
                    RawContents = translation.RawContents,
                    Notes = translation.Notes,
                    Content = translation.Content,
                };
                _context.Add(sqlTranslation);
            }
            _context.SaveChanges();
        }
        private void InsertSources()
        {
            var adminUser = _realm.All<chldr_data.Entities.User>().First();

            foreach (var source in _realm.All<Source>())
            {
                _context.Add(new Source()
                {
                    //SourceId = source._id.ToString(),
                    //UserId = adminUser._id.ToString(),
                    CreatedAt = source.CreatedAt.UtcDateTime,
                    UpdatedAt = source.UpdatedAt.UtcDateTime,
                    Name = source.Name,
                    Notes = source.Notes,
                });
            }

            _context.SaveChanges();
        }
        private void InsertUsers()
        {
            foreach (var user in _realm.All<chldr_data.Entities.User>())
            {
                _context.Add(
                    new User()
                    {
                        //UserId = user._id.ToString(),
                        CreatedAt = user.CreatedAt.UtcDateTime,
                        UpdatedAt = user.UpdatedAt.UtcDateTime,
                        //AccountStatus = (sbyte)user.Status,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Patronymic = user.Patronymic,
                        ImagePath = user.ImagePath,
                        Rate = user.Rate,
                    });
            }

            _context.SaveChanges();
        }
        internal void InsertLanguages()
        {
            var languages = _realm.All<Language>();
            var adminUser = _realm.All<User>().First();

            foreach (var language in languages)
            {
                _context.Languages.Add(new Language()
                {
                    //LanguageId = language._id.ToString(),
                    CreatedAt = language.CreatedAt.UtcDateTime,
                    UpdatedAt = language.UpdatedAt.UtcDateTime,
                    Name = language.Name,
                    //UserId = adminUser._id.ToString(),
                    Code = language.Code,
                });
            }
            _context.SaveChanges();
        }
        internal void InsertEntries()
        {
            foreach (var entry in _realm.All<Entry>())
            {
                var sqlEntry = new Entry()
                {
                //    EntryId = entry._id.ToString(),
                //    UserId = entry.User._id.ToString(),
                    CreatedAt = entry.CreatedAt.UtcDateTime,
                    UpdatedAt = entry.UpdatedAt.UtcDateTime,
                    Rate = entry.Rate,
                    RawContents = entry.RawContents,
                    //SourceId = entry.Source._id.ToString(),
                    Type = entry.Type,
                };
                _context.Entries.Add(sqlEntry);

                switch ((EntryType)entry.Type)
                {
                    case EntryType.Word:
                        var word = new Word()
                        {
                            //EntryId = entry._id.ToString(),
                            //WordId = entry.Word._id.ToString(),
                            //Content = entry.Word.Content,
                            //Notes = entry.Word.Notes,
                            //PartOfSpeech = entry.Word.PartOfSpeech,
                        };
                        _context.Words.Add(word);

                        break;
                    case EntryType.Phrase:
                        var phrase = new Phrase()
                        {
                            //EntryId = entry._id.ToString(),
                            //PhraseId = entry.Phrase._id.ToString(),
                            //Content = entry.Phrase.Content,
                            //Notes = entry.Phrase.Notes,
                        };
                        _context.Phrases.Add(phrase);

                        break;
                    case EntryType.Text:
                        var text = new Text()
                        {
                            //EntryId = entry._id.ToString(),
                            //TextId = entry.Text._id.ToString(),
                            //Content = entry.Text.Content,
                        };
                        _context.Texts.Add(text);

                        break;
                }
            }
            _context.SaveChanges();
        }
    }
}
