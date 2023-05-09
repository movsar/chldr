using chldr_data.Entities;

namespace chldr_data.Models
{
    public class SourceModel : PersistentModelBase
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