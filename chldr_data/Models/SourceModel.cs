using chldr_data.Entities;

namespace chldr_data.Models
{
    public class SourceModel : PersistentModelBase
    {
        public string Name { get; }
        public string Notes { get; }

        public SourceModel(RealmSource source)
        {
            Name = source.Name;
            Notes = source.Notes;
        }
    }
}