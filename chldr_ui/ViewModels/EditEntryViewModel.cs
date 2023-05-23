using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class EditEntryViewModel : ViewModelBase
    {
        [Parameter]
        public string? EntryId {get;set;}
        internal EntryDto EntryDto { get; set; } = new WordDto();
        internal EntryType SelectedEntryType { get; set; } = EntryType.Word;
        internal void HandleEntryTypeChange(ChangeEventArgs e)
        {
            if (Enum.TryParse(e.Value?.ToString(), out EntryType selectedEntryType))
            {
                SelectedEntryType = selectedEntryType;

                // Initialize the corresponding DTO based on the selected entry type
                EntryDto = SelectedEntryType switch
                {
                    EntryType.Word => new WordDto(),
                    EntryType.Phrase => new PhraseDto(),
                    EntryType.Text => new TextDto(),
                    _ => null
                };
            }
        }
    }
}
