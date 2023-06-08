using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.Services;
using chldr_data.Writers;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

namespace chldr_data.Repositories
{
    public class RealmWordsRepository : RealmRepository<RealmWord, WordModel, WordDto>, IWordsRepository
    {
        private readonly WordChangeRequests _wordChangeRequests;

        public RealmWordsRepository(Realm context, WordChangeRequests wordChangeRequests) : base(context)
        {
            _wordChangeRequests = wordChangeRequests;
        }

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

        public override async Task Add(string userId, WordDto dto)
        {
            // Make a remote add request, if successful, add locally
            var response = _wordChangeRequests.Add(userId, dto);

            // TODO: Add to local database

            throw new NotImplementedException();
        }

        public override async Task Update(string userId, WordDto dto)
        {
            // Make a remote update request, if successful, update locally
            var response = _wordChangeRequests.Update(userId, dto);

            // TODO: Update in local database

            //    // Update
            //    var userDto = UserDto.FromModel(loggedInUser);
            //    var changeSets = await UpdateWord(userDto, wordDto);

            //    // Sync offline database

            //    await _syncService.Sync(changeSets);

            //    // Refresh UI with new object 
            //    // TODO: Fix this!
            //    //var entry = Database.Find<RealmEntry>(wordDto.EntryId);
            //    //OnEntryUpdated(WordModel.FromEntity(entry.Word));

            throw new NotImplementedException();

        }

        public override Task Delete(string userId, string entityId)
        {
            var response = _wordChangeRequests.Delete(userId, entityId);
            
            // TODO: Delete from local database

            throw new NotImplementedException();
        }
    }
}