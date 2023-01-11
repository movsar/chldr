using Data.Entities;
using Data.Services;
using Data.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;

namespace Data.Search
{
    internal abstract class SearchEngine
    {
        protected readonly DataAccess _dataAccess;

        protected async Task DirectSearch(string inputText, Expression<Func<EntryEntity, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                var realmInstance = _dataAccess.GetRealmInstance();

                var entries = realmInstance.All<EntryEntity>().Where(filter)
                                                        .AsEnumerable()
                                                        .OrderByDescending(entry => entry.Rate)
                                                        .ThenBy(entry => entry.RawContents.IndexOf(inputText))
                                                        .Take(limit);
                foreach (var entry in entries)
                {
                    resultingEntries.Add(new EntryModel(entry));
                }
            });

            var args = new SearchResultsModel(inputText, resultingEntries, SearchResultsModel.Mode.Direct);
            _dataAccess.GotNewSearchResults?.Invoke(args);
        }

        protected async Task ReverseSearch(string inputText, Expression<Func<TranslationEntity, bool>> filter, int limit)
        {
            var resultingEntries = new List<EntryModel>();

            await Task.Run(() =>
            {
                var realmInstance = _dataAccess.GetRealmInstance();

                var translations = realmInstance.All<TranslationEntity>()
                                                                   .Where(filter)
                                                                   .AsEnumerable()
                                                                   .OrderBy(translation => translation.Content.IndexOf(inputText))
                                                                   .Take(limit);
                foreach (var translation in translations)
                {
                    resultingEntries.Add(new EntryModel(translation.Entry));
                }
            });

            var args = new SearchResultsModel(inputText, resultingEntries, SearchResultsModel.Mode.Reverse);
            _dataAccess.GotNewSearchResults?.Invoke(args);
        }
        public SearchEngine(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
    }
}
