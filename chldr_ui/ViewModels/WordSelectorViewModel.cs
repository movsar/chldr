using chldr_data.DatabaseObjects.Models;
using chldr_utils.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace chldr_ui.ViewModels
{
    public class WordSelectorViewModel : ViewModelBase
    {
        public List<EntryModel> Entries { get; set; }
        internal string? SearchQuery { get; set; }
        internal ElementReference SearchInputReference { get; set; }

        public void Search(ChangeEventArgs evgentArgs)
        {
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            Entries = ContentStore.Find(inputText).ToList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await SearchInputReference.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
