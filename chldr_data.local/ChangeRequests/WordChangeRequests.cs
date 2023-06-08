﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.ResponseTypes;
using GraphQL;
using chldr_utils.Interfaces;
using chldr_data.Interfaces;

namespace chldr_data.Writers
{
    public class WordChangeRequests : IChangeRequest
    {
        private readonly IGraphQLRequestSender _graphQLRequestSender;
        public WordChangeRequests(IGraphQLRequestSender graphQLRequestSender)
        {
            _graphQLRequestSender = graphQLRequestSender;
        }
        public async Task<IEnumerable<ChangeSetModel>> Update(string userId, WordDto wordDto)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation updateWord(userId: String!, $wordDto: WordDtoInput!) {
                          updateWord(userId: $userId, wordDto: $wordDto) {
                            success
                            errorMessage
                            changeSets {
                                changeSetId
                                recordId
                                recordChanges
                                recordType
                                operation
                                userId
                            }
                          }
                        }
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { userId, wordDto }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<UpdateResponse>(request, "updateWord");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            return response.Data.ChangeSets.Select(c => ChangeSetModel.FromDto(c)).ToList();
        }

        public async Task<IEnumerable<ChangeSetModel>> Add(string userId, WordDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ChangeSetModel>> Delete(string userId, string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
