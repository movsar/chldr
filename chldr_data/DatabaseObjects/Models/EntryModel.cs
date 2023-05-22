
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.DatabaseEntities;

namespace chldr_data.DatabaseObjects.Models
{
    public abstract class EntryModel : IEntry
    {
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
        public SourceModel Source { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        public string EntryId { get; internal set; }
        public abstract string Content { get; set; }
        public abstract DateTimeOffset CreatedAt { get; set; }
        public abstract DateTimeOffset UpdatedAt { get; set; }
        public string? SourceId => Source.SourceId;
    }
}
