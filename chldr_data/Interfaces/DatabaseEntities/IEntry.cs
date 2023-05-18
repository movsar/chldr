namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IEntry
    {
        string EntryId { get; }
        int Rate { get; }
        int Type { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
