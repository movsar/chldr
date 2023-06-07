using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.local.RealmEntities;
using chldr_data.Enums.WordDetails;
using chldr_data.Models;
using chldr_utils.Models;
using chldr_utils.Services;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using chldr_data.Interfaces;

namespace chldr_data.local.Services
{
    public class SearchService : ISearchService
    {
        private Realm Database => Realm.GetInstance(RealmDataProvider.OfflineDatabaseConfiguration);
        public event Action<SearchResultModel>? GotNewSearchResult;
        public static EntryModel FromEntity(RealmEntry entry)
        {
            return EntryModelFactory.CreateEntryModel(entry,
                            entry?.Word,
                            entry?.Phrase,
                            entry?.Text,
                            entry.Source,
                            entry.Translations.Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t))
                        );
        }
        private static IEnumerable<EntryModel> SortDirectSearchEntries(string inputText, IEnumerable<EntryModel> entries)
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
        Expression<Func<RealmEntry, bool>> EntryFilter(string inputText) => entry => entry.RawContents.Contains(inputText);
        protected async Task DirectSearch(string inputText, Expression<Func<RealmEntry, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                using var realmInstance = Database;

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

            });

            var args = new SearchResultModel(inputText, SortDirectSearchEntries(inputText, resultingEntries).ToList(), SearchResultModel.Mode.Direct);
            GotNewSearchResult?.Invoke(args);
        }
        protected async Task ReverseSearch(string inputText, Expression<Func<RealmTranslation, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                using var realmInstance = Database;

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
            });

            var args = new SearchResultModel(inputText, SortReverseSearchEntries(inputText, resultingEntries).ToList(), SearchResultModel.Mode.Reverse);
            GotNewSearchResult?.Invoke(args);
        }

        Expression<Func<RealmEntry, bool>> StartsWithFilter(string inputText) => translation => translation.RawContents.Contains(inputText);
        Expression<Func<RealmTranslation, bool>> TranslationFilter(string inputText) => entry => entry.RawContents.Contains(inputText);

        public async Task FindAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            var logger = new ConsoleService("GotNewSearchResults", true);
            logger.StartSpeedTest();

            inputText = inputText.Replace("1", "Ӏ").ToLower();

            if (inputText.Length < 3)
            {
                await DirectSearch(inputText, StartsWithFilter(inputText), 50);
            }
            else if (inputText.Length >= 3)
            {
                await DirectSearch(inputText, EntryFilter(inputText), 100);

                await ReverseSearch(inputText, TranslationFilter(inputText), 100);
            }
            logger.StopSpeedTest($"FindAsync finished");
        }



        public List<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();
            var realm = Database.All<RealmEntry>();

            var entries = Database.All<RealmEntry>().AsEnumerable()
              .Where(entry => entry.Rate > 0)
              .OrderBy(x => randomizer.Next(0, 70000))
              .Take(50)
              .OrderBy(entry => entry.GetHashCode())
              .Select(entry => FromEntity(entry))
              .ToList();

            return entries;
        }

        public List<EntryModel> GetWordsToFiddleWith()
        {
            var words = Database.All<RealmWord>().Where(w => w.PartOfSpeech == (int)PartOfSpeech.Verb);

            var entries = words.AsEnumerable().Select(w => w.Entry);

            var entriesToReturn = entries
              .Take(5)
              .Select(entry => FromEntity(entry));

            return entriesToReturn.ToList();
        }

        public List<EntryModel> GetEntriesOnModeration()
        {
            var entries = Database.All<RealmEntry>().AsEnumerable()
                .Where(entry => entry.Rate < UserModel.EnthusiastRateRange.Lower)
                .Take(50)
                .Select(entry => FromEntity(entry))
                .ToList();

            return entries;
        }
    }
}
