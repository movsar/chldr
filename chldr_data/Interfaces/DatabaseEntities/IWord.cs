namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IWord : IEntity
    {
        string Content { get;}
        string? Notes { get; }
        string WordId { get; }
    }
}
