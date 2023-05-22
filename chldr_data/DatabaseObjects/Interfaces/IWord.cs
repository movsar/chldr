namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface IWord
    {
        string Content { get; }
        string? Notes { get; }
        string WordId { get; }
    }
}
