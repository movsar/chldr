﻿using chldr_shared.Models;
using chldr_utils;
using chldr_utils.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class SearchQueryViewModel : ViewModelBase
    {
        internal string? SearchQuery { get; set; }
        internal ElementReference SearchInputReference { get; set; }
        internal FiltrationFlags FiltrationFlags = new FiltrationFlags();

        internal void ToggleOnModerationFlag()
        {
            ContentStore.LoadEntriesOnModeration();
            //FiltrationFlags.OnModeration = !FiltrationFlags.OnModeration;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (ContentStore.CachedSearchResults.Count > 0)
            {
                SearchQuery = ContentStore.CachedSearchResults.TakeLast(1).First().SearchQuery;
            }
        }

        // Called when something is typed into search input
        public void Search(ChangeEventArgs evgentArgs)
        {
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            ContentStore.Search(inputText, FiltrationFlags);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await SearchInputReference.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        public void LoadRandomEntries()
        {
            try
            {
                ContentStore.LoadRandomEntries();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while showing randoms");
            }
        }
    }
}