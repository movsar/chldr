namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IPhraseEntity : IPhrase, IEntity
    {
        string EntryId { get; }
    }
}
