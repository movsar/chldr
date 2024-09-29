using domain.DatabaseObjects.Interfaces;

namespace domain.DatabaseObjects.Models
{
    public class SourceModel : IEntity
    {
        public SourceModel() { }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string SourceId { get; set; }
        public string UserId { get; set; }
        public static SourceModel FromEntity(ISource source)
        {
            return new SourceModel()
            {
                SourceId = source.SourceId,
                UserId = source.UserId!,
                Name = source.Name,
                Notes = source.Notes ?? string.Empty,
            };
        }
    }
}