using chldr_data.Entities;
using Newtonsoft.Json;

namespace chldr_data.ResponseTypes
{
    //[Serializable]
    public class UpdateResponse : MutationResponse
    {
        //[JsonProperty("entry")]
        public SqlEntry Entry { get; set; }
    }
}
