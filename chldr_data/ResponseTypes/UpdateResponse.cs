using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.ResponseTypes
{
    public class UpdateResponse : MutationResponse
    {
        public List<ChangeSetDto> ChangeSets { get; } = new List<ChangeSetDto>();
    }
}
