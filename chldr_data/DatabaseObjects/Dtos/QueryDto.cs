using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class QueryDto : IQuery
    {
        public string Content { get;set; }
        public string QueryId { get;set; }
        public DateTimeOffset CreatedAt { get;set; }
        public DateTimeOffset UpdatedAt { get;set; }
    }
}
