using domain.DatabaseObjects.Dtos;
namespace domain.Responses
{
    public class InsertResponse : UpdateResponse
    {
        public DateTimeOffset CreatedAt { get; set; }
    }
}
