namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IWord
    {
        string Content { get; }
        string? Notes { get; }
        string WordId { get; }
    }
}
