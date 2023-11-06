using chldr_data.DatabaseObjects.Models;
using chldr_data.realm.RealmEntities;
using Realms;
using System.Linq.Expressions;

namespace chldr_data.realm.Services
{
    internal static class RealmSearchHelper
    {
        internal static EntryModel FromEntity(RealmEntry entry)
        {
            return EntryModel.FromEntity(
                entry,
                entry.Source,
                entry.Translations,
                entry.Sounds
            );
        }

        private static IEnumerable<EntryModel> SortReverseSearchEntries(string inputText, IEnumerable<EntryModel> entries)
        {
            // Entry.Translation.Content => Equal, StartsWith, Rest

            var equalTo = new List<EntryModel>();
            var startsWith = new List<EntryModel>();
            var rest = new List<EntryModel>();

            foreach (var entry in entries)
            {
                foreach (var translation in entry.Translations)
                {
                    var translationContent = entry.Content.ToLower();

                    if (translationContent.Equals(inputText))
                    {
                        equalTo.Add(entry);
                    }
                    else if (translationContent.StartsWith(inputText))
                    {
                        startsWith.Add(entry);
                    }
                    else
                    {
                        rest.Add(entry);
                    }
                }
            }

            var orderedStartsWith = startsWith.OrderBy(e => e.Content).ToList();
            var orderedRest = rest.OrderBy(e => e.Content).ToList();

            var combined = equalTo.Union(orderedStartsWith.Union(orderedRest));
            return combined;
        }
        public static Expression<Func<RealmEntry, bool>> EntryFilter(string inputText) => entry => entry.RawContents.Contains(inputText);
        public static List<EntryModel> DirectSearch(Realm realmInstance, string inputText, Expression<Func<RealmEntry, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();
            var entries = realmInstance.All<RealmEntry>().Where(filter)
                                                    .Where(e => e.Rate > 0)
                                                    .AsEnumerable()
                                                    .OrderBy(e => e.RawContents.IndexOf(inputText))
                                                    .OrderByDescending(e => e.Rate)
                                                    .Take(limit)
                                                    .ToList();

            foreach (var entry in entries)
            {
                resultingEntries.Add(FromEntity(entry));
            }


            return resultingEntries;
        }
        public static List<EntryModel> ReverseSearch(Realm realmInstance, string inputText, Expression<Func<RealmTranslation, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();
            var translations = realmInstance.All<RealmTranslation>()
                                                               .Where(filter)
                                                               .Where(t => t.Rate > 0)
                                                               .AsEnumerable()
                                                               .OrderBy(translation => translation.Content.IndexOf(inputText))
                                                               .OrderByDescending(translation => translation.Rate)
                                                               .Take(limit)
                                                               .ToList();
            foreach (var translation in translations)
            {
                resultingEntries.Add(FromEntity(translation.Entry));
            }

            return resultingEntries;
        }

        public static Expression<Func<RealmEntry, bool>> StartsWithFilter(string inputText) => translation => translation.RawContents.Contains(inputText);
        public static Expression<Func<RealmTranslation, bool>> TranslationFilter(string inputText) => entry => entry.RawContents.Contains(inputText);

    }
}