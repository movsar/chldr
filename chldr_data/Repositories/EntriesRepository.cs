using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_utils.Models;
using chldr_utils.Services;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Realms.Sync.MongoClient;

namespace chldr_data.Repositories
{
    public class EntriesRepository<TEntryModel> : Repository where TEntryModel : EntryModel
    {
        public event Action<SearchResultModel>? GotNewSearchResult;

        public EntriesRepository(IRealmService realmService) : base(realmService) { }

        protected async Task DirectSearch(string inputText, Expression<Func<Entities.Entry, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                using var realmInstance = Database;

                var entries = realmInstance.All<Entry>().Where(filter)
                                                        .AsEnumerable()
                                                        //.OrderByDescending(entry => entry.Rate)
                                                        //.ThenBy(entry => entry.RawContents.IndexOf(inputText))
                                                        .Take(limit);
                foreach (var entry in entries)
                {
                    resultingEntries.Add(EntryModelFactory.CreateEntryModel(entry));
                }
            });

            var args = new SearchResultModel(inputText, resultingEntries.ToList(), SearchResultModel.Mode.Direct);
            GotNewSearchResult?.Invoke(args);
        }

        protected async Task ReverseSearch(string inputText, Expression<Func<Translation, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                using var realmInstance = Database;

                var translations = realmInstance.All<Translation>()
                                                                   .Where(filter)
                                                                   .AsEnumerable()
                                                                   //.OrderBy(translation => translation.Content.IndexOf(inputText))
                                                                   .Take(limit);
                foreach (var translation in translations)
                {
                    resultingEntries.Add(EntryModelFactory.CreateEntryModel(translation.Entry));
                }
            });

            var args = new SearchResultModel(inputText, resultingEntries.ToList(), SearchResultModel.Mode.Reverse);
            GotNewSearchResult?.Invoke(args);
        }

        Expression<Func<Entities.Entry, bool>> StartsWithFilter(string inputText) => translation => translation.RawContents.Contains(inputText);
        Expression<Func<Entities.Entry, bool>> EntryFilter(string inputText) => entry => entry.RawContents.Contains(inputText);
        Expression<Func<Translation, bool>> TranslationFilter(string inputText) => entry => entry.RawContents.Contains(inputText);

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

        public TEntryModel GetByEntryId(ObjectId entryId)
        {
            var entry = Database.All<Entry>().FirstOrDefault(e => e._id == entryId);
            if (entry == null)
            {
                throw new NullReferenceException();
            }

            var entryModel = EntryModelFactory.CreateEntryModel(entry) as TEntryModel;
            return entryModel!;
        }

        public List<TEntryModel> Take(int limit, int skip = 0)
        {
            var entries = Database.All<Entry>().AsEnumerable()
                .Skip(skip).Take(limit)
                .Select(e => EntryModelFactory.CreateEntryModel(e) as TEntryModel)
                .ToList();
            return entries;
        }

        public List<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();

            var entries = Database.All<Entry>().AsEnumerable()
              .Where(entry => entry.Rate > 0)
              .OrderBy(x => randomizer.Next(0, 70000))
              .Take(50)
              .OrderBy(entry => entry.GetHashCode())
              .Select(entry => EntryModelFactory.CreateEntryModel(entry))
              .ToList();

            return entries;
        }

        public List<EntryModel> GetEntriesOnModeration()
        {
            var entries = Database.All<Entry>().AsEnumerable()
                .Where(entry => entry.Rate < UserModel.EnthusiastRateRange.Lower)
                .Select(entry => EntryModelFactory.CreateEntryModel(entry))
                .ToList();

            return entries;
        }

        public void RequestRandomEntries()
        {
            GotNewSearchResult?.Invoke(new SearchResultModel(GetRandomEntries()));
        }

        public void RequestEntriesOnModeration()
        {
            GotNewSearchResult?.Invoke(new SearchResultModel(GetEntriesOnModeration()));
        }
    }
}
