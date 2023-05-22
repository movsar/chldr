using chldr_data.DatabaseObjects.RealmEntities;
namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ISound
    {
        string FileName { get; set; }
        string SoundId { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
