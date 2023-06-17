using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class SoundModel : ISound
    {
        public string FileName { get; set; }
        public string SoundId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string EntryId { get; set; }

        public static SoundModel FromEntity(ISoundEntity entity)
        {
            return new SoundModel()
            {
                SoundId = entity.SoundId,
                EntryId = entity.EntryId,
                FileName = entity.FileName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }
    }
}
