namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IWord
    {
        string Content { get;}
        string? Notes { get; }
        string WordId { get; }
    }
}
