using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Interfaces
{
    public interface IPhraseEntity : IPhrase, IEntity
    {
        string EntryId { get; }
    }
}
