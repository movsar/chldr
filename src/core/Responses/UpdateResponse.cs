using core.DatabaseObjects.Models;

namespace core.Responses
{
    public class UpdateResponse
    {
        public IEnumerable<ChangeSetModel> ChangeSets { get; set; }
    }
}
