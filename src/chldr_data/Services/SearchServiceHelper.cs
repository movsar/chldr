using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Services
{
    public static class SearchServiceHelper
    {
        public static IEnumerable<EntryModel> SortDirectSearchEntries(string inputText, IEnumerable<EntryModel> entries)
        {
            // Entry.Content => Equal, StartsWith, Rest
            var equalTo = new List<EntryModel>();
            var startsWith = new List<EntryModel>();
            var rest = new List<EntryModel>();

            foreach (var entry in entries)
            {
                var entryContent = entry.Content.ToLower();

                if (entryContent.Equals(inputText))
                {
                    equalTo.Add(entry);
                }
                else if (entryContent.ToLower().StartsWith(inputText))
                {
                    startsWith.Add(entry);
                }
                else
                {
                    rest.Add(entry);
                }
            }

            var orderedStartsWith = startsWith.OrderBy(e => e.Content).ToList();
            var orderedRest = rest.OrderBy(e => e.Content).ToList();

            var combined = equalTo.Union(orderedStartsWith.Union(orderedRest));
            return combined;
        }
    }
}
