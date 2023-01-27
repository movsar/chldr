using chldr_data.Models;
using chldr_shared.Enums;
using chldr_shared.Services;
using MailKit.Search;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
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

        private void ContentStore_GotNewSearchResult(SearchResultModel searchResult)
        {
            if (_searchQuery == null || !_searchQuery.Equals(searchResult.SearchQuery))
            {
                Entries.Clear();
                _searchQuery = searchResult.SearchQuery;
            }

            Entries.AddRange(searchResult.Entries);

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
    }
}
