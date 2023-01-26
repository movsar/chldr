﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using chldr_shared.Stores;
using chldr_shared.Services;
using chldr_shared.Enums;
using chldr_data.Models;

namespace chldr_ui.ViewModels
{
    public partial class MainPageViewModel : ViewModelBase
    {
        [Inject] JsInterop? JsInteropFunctions { get; set; }

        #region Properties
        internal List<SearchResultModel> SearchResults { get; } = new();
        internal ElementReference SearchInputReference { get; set; }
        internal string SearchQuery { get; set; }
        #endregion

        #region Fields
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ContentStore.DatabaseInitialized += ContentStore_DatabaseInitialized;
            ContentStore.GotNewSearchResult += ContentStore_GotNewSearchResult;
        }

        #region EventHandlers
        private async void ContentStore_DatabaseInitialized()
        {
            if (EnvironmentService?.CurrentPlatform != Platforms.Web)
            {
                ContentStore.LoadRandomEntries();
            }
            else
            {
                await Task.Delay(500);
                await JsInteropFunctions!.ClickShowRandoms();
            }
        }

        private void ContentStore_GotNewSearchResult(SearchResultModel searchResult)
        {
            SearchResults.Add(searchResult);
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        // Called when something is typed into search input
        public void Search(ChangeEventArgs evgentArgs)
        {
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            SearchResults.Clear();
            ContentStore.Search(inputText);
        }
        #endregion

        #region Methods
        public void LoadRandomEntries()
        {
            SearchResults.Clear();

            try
            {
                ContentStore.LoadRandomEntries();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while showing randoms");
            }
        }
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await SearchInputReference.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            //PropertyChanged += async (sender, e) =>
            //{
            //    await InvokeAsync(() =>
            //    {
            //        StateHasChanged();
            //    });
            //};
            await base.OnInitializedAsync();
        }
        #endregion
    }
}
