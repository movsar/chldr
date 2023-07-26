using chldr_data.DatabaseObjects.Dtos;
namespace chldr_data.Responses
{
    public class UpdateResponse
    {
        public IEnumerable<ChangeSetDto> ChangeSets { get; set; }
    }
}
