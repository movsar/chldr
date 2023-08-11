using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Models;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class WordSelectorViewModel : ViewModelBase
    {
        public List<EntryModel> Entries { get; set; }
        internal string? SearchQuery { get; set; }
        internal ElementReference SearchInputReference { get; set; }
        internal string? SelectedEntryId { get; set; }

        [Parameter]
        public EventCallback<EntryModel> OnEntrySelected { get; set; }
        public async Task SelectEntry(EntryModel entry)
        {
            SelectedEntryId = entry.EntryId;
            await OnEntrySelected.InvokeAsync(entry);
        }
        public async Task Search(ChangeEventArgs evgentArgs)
        {
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            Entries = (await ContentStore.EntryService.FindAsync(inputText, new FiltrationFlags())).ToList();
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
