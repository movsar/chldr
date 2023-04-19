using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Models.Words;
using chldr_utils.Models;
using chldr_utils.Services;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace chldr_data.Repositories
{
    public class EntriesRepository<TEntryModel> : Repository where TEntryModel : EntryModel
    {
        public event Action<SearchResultModel>? GotNewSearchResult;
        public event Action<EntryModel>? EntryUpdated;
        public event Action<WordModel>? WordUpdated;
        public event Action<EntryModel>? EntryInserted;
        public EntriesRepository(IDataAccess dataAccess) : base(dataAccess) { }
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
        Expression<Func<Entry, bool>> EntryFilter(string inputText) => entry => entry.RawContents.Contains(inputText);
        protected async Task DirectSearch(string inputText, Expression<Func<Entities.Entry, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                using var realmInstance = Database;

                var entries = realmInstance.All<Entry>().Where(filter)
                                                        .Where(e => e.Rate > 0)
                                                        .AsEnumerable()
                                                        .OrderBy(e => e.RawContents.IndexOf(inputText))
                                                        .OrderByDescending(e => e.Rate)
                                                        .Take(limit)
                                                        .ToList();

                foreach (var entry in entries)
                {
                    resultingEntries.Add(EntryModelFactory.CreateEntryModel(entry));
                }
            });


            var args = new SearchResultModel(inputText, SortDirectSearchEntries(inputText, resultingEntries).ToList(), SearchResultModel.Mode.Direct);
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
                                                                   .Where(t => t.Rate > 0)
                                                                   .AsEnumerable()
                                                                   .OrderBy(translation => translation.Content.IndexOf(inputText))
                                                                   .OrderByDescending(translation => translation.Rate)
                                                                   .Take(limit)
                                                                   .ToList();
                foreach (var translation in translations)
                {
                    resultingEntries.Add(EntryModelFactory.CreateEntryModel(translation.Entry));
                }
            });

            var args = new SearchResultModel(inputText, SortReverseSearchEntries(inputText, resultingEntries).ToList(), SearchResultModel.Mode.Reverse);
            GotNewSearchResult?.Invoke(args);
        }

        Expression<Func<Entry, bool>> StartsWithFilter(string inputText) => translation => translation.RawContents.Contains(inputText);
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

        public TEntryModel GetByEntryId(string entryId)
        {
            var entry = Database.All<Entry>().FirstOrDefault(e => e.EntryId == entryId);
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

        public void Delete(string Id)
        {
            var entry = Database.Find<Entry>(Id);
            if (entry == null)
            {
                return;
            }

            Database.Write(() =>
            {
                foreach (var translation in entry.Translations)
                {
                    Database.Remove(translation);
                }
                switch ((EntryType)entry.Type)
                {
                    //case EntryType.Word:
                    //    Database.Remove(entry.Word);
                    //    break;
                    //case EntryType.Phrase:
                    //    Database.Remove(entry.Phrase);
                    //    break;
                    //case EntryType.Text:
                    //    Database.Remove(entry.Text);
                    //    break;
                    //default:
                    //    break;
                }
                Database.Remove(entry);
            });
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

        public List<EntryModel> GetWordsToFiddleWith()
        {
            var words = Database.All<Word>().Where(w => w.PartOfSpeech == (int)PartOfSpeech.Verb);

            var entries = words.AsEnumerable().Select(w => w.Entry);

            var entriesToReturn = entries
              .Take(5)
              .Select(entry => EntryModelFactory.CreateEntryModel(entry));

            return entriesToReturn.ToList();
        }

        public List<EntryModel> GetEntriesOnModeration()
        {
            var entries = Database.All<Entry>().AsEnumerable()
                .Where(entry => entry.Rate < UserModel.EnthusiastRateRange.Lower)
                .Take(50)
                .Select(entry => EntryModelFactory.CreateEntryModel(entry))
                .ToList();

            return entries;
        }

        protected void OnEntryInserted(EntryModel entry)
        {
            EntryInserted?.Invoke(entry);
        }
        protected void OnEntryUpdated(WordModel word)
        {
            WordUpdated?.Invoke(word);
        }
    }
}
