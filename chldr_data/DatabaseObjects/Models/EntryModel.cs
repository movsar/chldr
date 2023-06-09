
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public abstract class EntryModel : IEntry
    {
        public string EntryId { get; internal set; }
        public string UserId { get; set; }
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        public abstract string Content { get; set; }
        public SourceModel Source { get; set; }
        public string? SourceId => Source.SourceId;
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
        public abstract DateTimeOffset CreatedAt { get; set; }
        public abstract DateTimeOffset UpdatedAt { get; set; }

    }
}
