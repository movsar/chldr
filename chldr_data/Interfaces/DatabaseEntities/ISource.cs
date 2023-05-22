namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface ISource
    {
        string Name { get; set; }
        string? Notes { get; set; }
        string? UserId { get;}
        string SourceId { get; set; }
    }
}
