using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class SourceModel : IEntity
    {
        private SourceModel() { }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string? SourceId { get; internal set; }
        public string? UserId { get; internal set; }
        public static SourceModel FromEntity(ISource source)
        {
            return new SourceModel()
            {
                SourceId = source.SourceId,
                UserId = source.UserId,
                Name = source.Name,
                Notes = source.Notes ?? string.Empty,
            };
        }
    }
}