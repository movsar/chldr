using chldr_data.Entities;

namespace chldr_data.Models
{
    public class SourceModel : ModelBase
    {
        public string Name { get; }
        public string Notes { get; }

        public SourceModel(Source source) : base(source)
        {
            Name = source.Name;
            Notes = source.Notes;
        }
    }
}