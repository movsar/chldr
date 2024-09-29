namespace domain.DatabaseObjects.Interfaces
{
    public interface ISource
    {
        string Name { get; set; }
        string? Notes { get; set; }
        string? UserId { get; }
        string SourceId { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
