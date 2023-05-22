using chldr_data.DatabaseObjects.DatabaseEntities;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class SourceDto : ISource
    {
        public string? SourceId { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string? UserId { get; set; }
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
