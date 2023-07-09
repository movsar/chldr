﻿using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.local.RealmEntities;
using chldr_data.Enums.WordDetails;
using chldr_data.Models;
using chldr_utils.Models;
using chldr_utils.Services;
using Realms;
using System.Linq.Expressions;
using chldr_data.Interfaces;

namespace chldr_data.local.Services
{
    public class SearchService : ISearchService
    {
        private Realm Database => Realm.GetInstance(RealmDataProvider.OfflineDatabaseConfiguration);
        public event Action<SearchResultModel>? GotNewSearchResult;
        public static EntryModel FromEntity(RealmEntry entry)
        {
            return EntryModel.FromEntity(
                entry,
                entry.Source,
                entry.Translations,
                entry.Sounds
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

        public List<EntryModel> GetLatestEntries()
        {
            var entries = Database.All<RealmEntry>().AsEnumerable().OrderByDescending(e => e.CreatedAt).Take(100);

            var entriesToReturn = entries.Select(entry => FromEntity(entry));

            return entriesToReturn.ToList();
        }

        public List<EntryModel> GetWordsToFiddleWith()
        {
            var words = Database.All<RealmEntry>().Where(w => w.Subtype == (int)WordType.Verb);

            var entries = words.AsEnumerable();

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

        public List<EntryModel> Find(string startsWith, int limit)
        {
            var inputText = startsWith.Replace("1", "Ӏ").ToLower();

            var resultingEntries = new List<EntryModel>();

            using var realmInstance = Database;

            var entries = realmInstance.All<RealmEntry>()
                                                    .Where(StartsWithFilter(inputText))
                                                    .Where(e => e.Rate > 0)
                                                    .AsEnumerable()
                                                    .Take(limit);

            foreach (var entry in entries)
            {
                resultingEntries.Add(FromEntity(entry));
            }

            return resultingEntries;
        }
    }
}