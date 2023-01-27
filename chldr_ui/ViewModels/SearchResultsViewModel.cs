using chldr_data.Models;
using chldr_shared.Enums;
using chldr_shared.Services;
using chldr_utils;
using MailKit.Search;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private string _searchQuery = string.Empty;
        public List<EntryModel> Entries { get; } = new();

        protected override void OnInitialized()
        {
            ContentStore.GotNewSearchResult += ContentStore_GotNewSearchResult;
            base.OnInitialized();
        }

        private async void ContentStore_GotNewSearchResult(SearchResultModel searchResult)
        {
            var logger = new LoggerService("GotNewSearchResults", true);
            logger.StartSpeedTest();

            if (_searchQuery == null || !_searchQuery.Equals(searchResult.SearchQuery))
            {
                Entries.Clear();
                _searchQuery = searchResult.SearchQuery;

                await CallStateHasChangedAsync();
                logger.StopSpeedTest($"Finished setting up");
            }

            logger.StartSpeedTest();
            Entries.AddRange(searchResult.Entries);
            logger.StopSpeedTest($"Finished adding entries to the collection");

            logger.StartSpeedTest();
            await CallStateHasChangedAsync();
            logger.StopSpeedTest($"Finished rendering");
        }
    }
}
