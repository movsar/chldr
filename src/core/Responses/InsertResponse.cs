using core.DatabaseObjects.Dtos;
namespace core.Responses
{
    public class InsertResponse : UpdateResponse
    {
        public DateTimeOffset CreatedAt { get; set; }
    }
}
