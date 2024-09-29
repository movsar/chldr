using domain.DatabaseObjects.Models;

namespace domain.Responses
{
    public class UpdateResponse
    {
        public IEnumerable<ChangeSetModel> ChangeSets { get; set; }
    }
}
