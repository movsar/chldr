using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using chldr_utils;
using GraphQL;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmWordsRepository : RealmRepository<RealmWord, WordModel, WordDto>, IWordsRepository
    {
        public RealmWordsRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

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
            var word = _dbContext.Find<RealmEntry>(entryId)!.Word;
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }

        public List<WordModel> GetRandomWords(int limit)
        {
            var words = _dbContext.All<RealmWord>().AsEnumerable().Take(limit);
            return words.Select(w => FromEntity(w)).ToList();
        }
        public override WordModel Get(string entityId)
        {
            var word = _dbContext.All<RealmWord>().FirstOrDefault(w => w.WordId == entityId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(word);
        }

        public override async Task Insert(string userId, WordDto dto)
        {
            // Make a remote add request, if successful, add locally
            //var response = _wordChangeRequests.Add(userId, dto);

            // TODO: Add to local database

            throw new NotImplementedException();
        }

        public override Task Delete(string userId, string entityId)
        {
            //var response = _wordChangeRequests.Delete(userId, entityId);

            // TODO: Delete from local database

            throw new NotImplementedException();
        }

        private void UpdateRealmEntities(string userId, WordDto updatedWordDto, ITranslationsRepository translationsRepository)
        {
            var existingWordDto = WordDto.FromModel(Get(updatedWordDto.WordId));

            // Apply changes to the entry entity
            var entryChanges = Change.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges<RealmEntry>(updatedWordDto.EntryId, entryChanges);
            }

            // Apply changes to the word entity
            var wordChanges = Change.GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                ApplyChanges<RealmWord>(updatedWordDto.WordId, wordChanges);
            }

            // Update translations
            var existingTranslationIds = existingWordDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedWordDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedWordDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingWordDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedWordDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var translationDto in insertedTranslations)
            {
                translationsRepository.Insert(userId, translationDto);
            }

            foreach (var translationDto in deletedTranslations)
            {
                translationsRepository.Delete(userId, translationDto.TranslationId);
            }

            foreach (var translationDto in updatedTranslations)
            {
                translationsRepository.Update(userId, translationDto);
            }
        }
        public async Task Update(string userId, WordDto wordDto, ITranslationsRepository translationsRepository)
        {
            // Make a remote update request, if successful, update locally
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation updateWord($userId: String!, $wordDto: WordDtoInput!) {
                          updateWord(userId: $userId, wordDto: $wordDto) {
                            success
                            errorMessage
                          }
                        }
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { userId, wordDto }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<MutationResponse>(request, "updateWord");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            UpdateRealmEntities(userId, wordDto, translationsRepository);

            //    // TODO: Fix this!
            //    //var entry = Database.Find<RealmEntry>(wordDto.EntryId);
            //    //OnEntryUpdated(WordModel.FromEntity(entry.Word));
        }
    }
}