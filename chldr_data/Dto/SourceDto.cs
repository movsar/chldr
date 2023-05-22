using chldr_data.Interfaces.DatabaseEntities;
using chldr_data.Models;

namespace chldr_data.Dto
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
