namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface IPhraseEntity : IPhrase, IEntity
    {
        string EntryId { get; }
    }
}
