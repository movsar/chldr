using domain.Enums;

namespace domain.Models
{
    public class EntryFilters
    {
        public string? StartsWith { get; set; } = null;
        public bool? IncludeOnModeration { get; set; }
        public EntryType[]? EntryTypes { get; set; }
    }
}
