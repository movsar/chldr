using chldr_data.Entities;
using chldr_data.Services;
using chldr_data.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using chldr_data.Interfaces;
using Entry = chldr_data.Entities.Entry;
using chldr_data.Factories;

namespace chldr_data.Search
{
    internal abstract class SearchEngine
    {
        protected readonly DataAccess _dataAccess;

        protected async Task DirectSearch(string inputText, Expression<Func<Entities.Entry, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                var realmInstance = RealmService.GetRealm();

                var entries = realmInstance.All<Entry>().Where(filter)
                                                        .AsEnumerable()
                                                        .OrderByDescending(entry => entry.Rate)
                                                        .ThenBy(entry => entry.RawContents.IndexOf(inputText))
                                                        .Take(limit);
                foreach (var entry in entries)
                {
                    resultingEntries.Add(EntryModelFactory.CreateEntryModel(entry));
                }
            });

            var args = new SearchResultsModel(inputText, resultingEntries, SearchResultsModel.Mode.Direct);
            _dataAccess.OnNewResults(args);
        }

        protected async Task ReverseSearch(string inputText, Expression<Func<Translation, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                var realmInstance = RealmService.GetRealm();

                var translations = realmInstance.All<Translation>()
                                                                   .Where(filter)
                                                                   .AsEnumerable()
                                                                   .OrderBy(translation => translation.Content.IndexOf(inputText))
                                                                   .Take(limit);
                foreach (var translation in translations)
                {
                    resultingEntries.Add(EntryModelFactory.CreateEntryModel(translation.Entry));
                }
            });

            var args = new SearchResultsModel(inputText, resultingEntries, SearchResultsModel.Mode.Reverse);
            _dataAccess.OnNewResults(args);
        }
        public SearchEngine(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
    }
}
