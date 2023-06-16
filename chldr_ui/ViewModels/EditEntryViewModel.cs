using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class EditEntryViewModel : ViewModelBase
    {
        [Parameter]
        public string? EntryId { get; set; }
        internal EntryDto EntryDto { get; set; } = new EntryDto();
        internal EntryType SelectedEntryType { get; set; } = EntryType.Word;
        internal void HandleEntryTypeChange(ChangeEventArgs e)
        {
            if (Enum.TryParse(e.Value?.ToString(), out EntryType selectedEntryType))
            {
                SelectedEntryType = selectedEntryType;

                // Initialize the corresponding DTO based on the selected entry type
                EntryDto = new EntryDto();
            }
        }
    }
}
