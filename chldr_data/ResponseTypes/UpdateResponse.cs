using chldr_data.Entities;

namespace chldr_data.ResponseTypes
{
    public class UpdateResponse : MutationResponse
    {
        public SqlEntry Entry { get; set; }
    }
}
