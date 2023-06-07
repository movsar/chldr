using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.ResponseTypes;
using GraphQL;
using chldr_utils.Interfaces;

namespace chldr_data.Writers
{
    public class WordChangeRequests
    {
        private readonly IGraphQLRequestSender _graphQLRequestSender;
        public WordChangeRequests(IGraphQLRequestSender graphQLRequestSender)
        {
            _graphQLRequestSender = graphQLRequestSender;
        }
        public async Task<List<ChangeSetModel>> UpdateWord(string userId, WordDto wordDto)
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

        //public async Task Update(UserModel loggedInUser, WordDto wordDto)
        //{
        //    // Update
        //    var userDto = UserDto.FromModel(loggedInUser);
        //    var changeSets = await UpdateWord(userDto, wordDto);

        //    // Sync offline database
            
        //    await _syncService.Sync(changeSets);

        //    // Refresh UI with new object 
        //    // TODO: Fix this!
        //    //var entry = Database.Find<RealmEntry>(wordDto.EntryId);
        //    //OnEntryUpdated(WordModel.FromEntity(entry.Word));
        //}
    }
}
