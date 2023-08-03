using chldr_data.Enums;

namespace chldr_data.Models
{
    public class FiltrationFlags
    {
        public bool OnModeration = false;
        public bool GroupWithSubEntries = false;
        public string? StartsWith = null;
        public EntryType[] EntryTypes = new EntryType[] { EntryType.Text, EntryType.Word, EntryType.Phrase };
    }
}
