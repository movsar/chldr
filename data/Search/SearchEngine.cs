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

namespace Data.Search
{
    internal abstract class SearchEngine
    {
        protected readonly RealmDataAccessService _dataAccess;

        protected async Task DirectSearch(string inputText, Expression<Func<EntryEntity, bool>> filter, int limit)
        {
            var resultingEntries = await Task.Run(() =>
            {
                var realmInstance = _dataAccess.GetRealmInstance();
                var results = new SearchResultsModel(SearchResultsModel.Mode.Direct);

                var entries = realmInstance.All<EntryEntity>().Where(filter)
                                                        .AsEnumerable()
                                                        .OrderByDescending(entry => entry.Rate)
                                                        .ThenBy(entry => entry.RawContents.IndexOf(inputText))
                                                        .Take(limit);
                foreach (var entry in entries)
                {
                    results.Entries.Add(new EntryModel(entry));
                }

                return results;
            });

            _dataAccess.NewSearchResults?.Invoke(inputText, resultingEntries);
        }

        protected async Task ReverseSearch(string inputText, Expression<Func<TranslationEntity, bool>> filter, int limit)
        {
            var resultingEntries = await Task.Run(() =>
            {
                var realmInstance = _dataAccess.GetRealmInstance();
                var results = new SearchResultsModel(SearchResultsModel.Mode.Direct);

                var translations = realmInstance.All<TranslationEntity>()
                                                                   .Where(filter)
                                                                   .AsEnumerable()
                                                                   .OrderBy(translation => translation.Content.IndexOf(inputText))
                                                                   .Take(limit);
                foreach (var translation in translations)
                {
                    results.Entries.Add(new EntryModel(translation.Entry));
                }

                return results;
            });

            _dataAccess.NewSearchResults?.Invoke(inputText, resultingEntries);
        }
        public SearchEngine(RealmDataAccessService dataAccess)
        {
            _dataAccess = dataAccess;
        }
    }
}
