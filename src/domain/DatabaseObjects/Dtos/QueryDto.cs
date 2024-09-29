using domain.DatabaseObjects.Interfaces;

namespace domain.DatabaseObjects.Dtos
{
    public class QueryDto : IQuery
    {
        public string QueryId { get;set; } = Guid.NewGuid().ToString();
        public string Content { get;set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
