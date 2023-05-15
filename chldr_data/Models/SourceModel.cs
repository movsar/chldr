using chldr_data.Entities;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models
{
    public class SourceModel : IEntity
    {
        public string Name { get; }
        public string Notes { get; }
        public string? SourceId { get; internal set; }

        public SourceModel(RealmSource source)
        {
            SourceId = source.SourceId;
            Name = source.Name;
            Notes = source.Notes;
        }
    }
}