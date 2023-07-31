namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IQuery
    {
        string Content { get; set; }
        string QueryId { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
