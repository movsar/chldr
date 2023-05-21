using chldr_data.Dto;
using chldr_data.Models;

namespace chldr_data.ResponseTypes
{
    public class UpdateResponse : MutationResponse
    {
        public List<ChangeSetDto> ChangeSets { get; } = new List<ChangeSetDto>();
    }
}
