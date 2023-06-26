using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.Responses
{
    public class InsertResponse
    {
        public DateTimeOffset CreatedAt { get; set; }
        public IEnumerable<ChangeSetDto> ChangeSets { get; set; }
    }
}
