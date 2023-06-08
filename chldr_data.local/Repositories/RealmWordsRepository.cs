﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_data.Services;
using chldr_tools;
using chldr_utils.Interfaces;
using chldr_utils;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

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

        public override async Task Update(string userId, WordDto wordDto)
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

            // TODO: Update in local database
            //    await _syncService.Sync(changeSets);
            // Refresh UI with new object 
            
            //    // TODO: Fix this!
            //    //var entry = Database.Find<RealmEntry>(wordDto.EntryId);
            //    //OnEntryUpdated(WordModel.FromEntity(entry.Word));
        }

        public override Task Delete(string userId, string entityId)
        {
            //var response = _wordChangeRequests.Delete(userId, entityId);

            // TODO: Delete from local database

            throw new NotImplementedException();
        }

        public Task Update(string userId, WordDto updatedWordDto, ITranslationsRepository translationsRepository)
        {
            throw new NotImplementedException();
        }
    }
}