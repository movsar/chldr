using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;

namespace domain.DatabaseObjects.Dtos
{
    public class SourceDto : ISource
    {
        public string SourceId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public static SourceDto FromModel(SourceModel source)
        {
            return new SourceDto()
            {
                UserId = source.UserId,
                SourceId = source.SourceId,
                Name = source.Name,
                Notes = source.Notes,
            };
        }

    }
}
