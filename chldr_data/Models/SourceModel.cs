using chldr_data.Entities;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models
{
    public class SourceModel : IEntity
    {
        public string Name { get; set; }
        public string Notes { get; set; }
        public string? SourceId { get; internal set; }
        public string? UserId { get; internal set; }

        public static SourceModel FromEntity(ISource source)
        {
            return new SourceModel()
            {
                SourceId = source.SourceId,
                Name = source.Name,
                Notes = source.Notes,
            };
        }
    }
}