using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Responses
{
    public class UpdateResponse
    {
        public IEnumerable<ChangeSetModel> ChangeSets { get; set; }
    }
}
