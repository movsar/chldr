using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.Models;
using chldr_data.ResponseTypes;
using GraphQL;
using chldr_data.Services;
using chldr_data.Interfaces;
using chldr_utils.Interfaces;
using chldr_data.local.Services;

namespace chldr_data.Writers
{
    public class WordsWriter
    {
        private readonly IGraphQLRequestSender _graphQLRequestSender;
        private readonly SyncService _syncService;

        public event Action<EntryModel>? EntryUpdated;
        public event Action<WordModel>? WordUpdated;
        public event Action<EntryModel>? EntryInserted;
        public WordsWriter(IGraphQLRequestSender graphQLRequestSender, SyncService syncService)
        {
            _graphQLRequestSender = graphQLRequestSender;
            _syncService = syncService;
        }
        public async Task<List<ChangeSetModel>> UpdateWord(UserDto userDto, WordDto wordDto)
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

            var response = await _graphQLRequestSender.SendRequestAsync<UpdateResponse>(request, "updateWord");
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
            
            await _syncService.Sync(changeSets);

            // Refresh UI with new object 
            // TODO: Fix this!
            //var entry = Database.Find<RealmEntry>(wordDto.EntryId);
            //OnEntryUpdated(WordModel.FromEntity(entry.Word));
        }
    }
}
