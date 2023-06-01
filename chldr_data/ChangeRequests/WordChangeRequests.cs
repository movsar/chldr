using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;
using chldr_data.ResponseTypes;

namespace chldr_data.ChangeRequests
{
    internal class WordChangeRequests
    {
        internal async Task<List<ChangeSetModel>> UpdateWord(UserDto userDto, WordDto wordDto)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation updateWord($userDto: UserDtoInput!, $wordDto: WordDtoInput!) {
                          updateWord(userDto: $userDto, wordDto: $wordDto) {
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
                Variables = new { userDto, wordDto }
            };

            var response = await DataAccess.RequestSender.SendRequestAsync<UpdateResponse>(request, "updateWord");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            return response.Data.ChangeSets.Select(c => ChangeSetModel.FromDto(c)).ToList();
        }

        public async Task Update(UserModel loggedInUser, WordDto wordDto)
        {
            // Update
            var userDto = UserDto.FromModel(loggedInUser);
            var changeSets = await UpdateWord(userDto, wordDto);

            // Sync offline database
            await Sync(changeSets);

            // Refresh UI with new object 
            var entry = Database.Find<RealmEntry>(wordDto.EntryId);
            OnEntryUpdated(WordModel.FromEntity(entry.Word));
        }
    }
}
