using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class QueryModel : IQuery
    {
        public string Content { get; set; }
        public string QueryId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public static QueryModel FromEntity(IQueryEntity entity)
        {
            return new QueryModel()
            {
                Content = entity.Content,
                QueryId = entity.QueryId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }
    }
}
