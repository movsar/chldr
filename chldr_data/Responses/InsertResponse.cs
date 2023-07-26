using chldr_data.DatabaseObjects.Dtos;
namespace chldr_data.Responses
{
    public class InsertResponse : UpdateResponse
    {
        public DateTimeOffset CreatedAt { get; set; }
    }
}
