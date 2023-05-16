using chldr_data.Entities;
using chldr_data.SqlEntities;
using Newtonsoft.Json;

namespace chldr_data.ResponseTypes
{
    public class UpdateResponse : MutationResponse
    {
        public SqlChangeSet ChangeSet { get; set; }
    }
}
