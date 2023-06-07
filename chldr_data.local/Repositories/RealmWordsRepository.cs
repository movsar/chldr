using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.Services;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

namespace chldr_data.Repositories
{
    public class RealmWordsRepository : RealmRepository<RealmWord, WordModel, WordDto>, IWordsRepository
    {
        public RealmWordsRepository(Realm context) : base(context)
        { }

        protected override RecordType RecordType => RecordType.Word;

        public static WordModel FromEntity(RealmWord word)
        {
            return WordModel.FromEntity(word.Entry,
                                    word.Entry.Word,
                                    word.Entry.Source,
                                    word.Entry.Translations
                                        .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        }
        public EntryModel GetByEntryId(string entryId)
        {
            var word = DbContext.Find<RealmEntry>(entryId)!.Word;
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }

        public List<WordModel> GetRandomWords(int limit)
        {
            var words = DbContext.All<RealmWord>().AsEnumerable().Take(limit);
            return words.Select(w => FromEntity(w)).ToList();
        }
        public override WordModel Get(string entityId)
        {
            var word = DbContext.All<RealmWord>().FirstOrDefault(w => w.WordId == entityId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }

        public override IEnumerable<ChangeSetModel> Add(string userId, WordDto dto)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, WordDto dto)
        {
            throw new NotImplementedException();
        }
    }
}