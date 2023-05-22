using chldr_data.DatabaseObjects.Dtos;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class NewEntryViewModel : ViewModelBase
    {
        internal EntryDto EntryDto;
        

        protected override void OnInitialized()
        {
            EntryDto = new WordDto(); // Initialize with a default DTO, such as WordDto
            base.OnInitialized();
    
        }

        internal void HandleEntryTypeChange(ChangeEventArgs e)
        {
            var entryType = e.Value.ToString();
            switch (entryType)
            {
                case "WordDto":
                    EntryDto = new WordDto();
                    break;
                case "PhraseDto":
                    EntryDto = new PhraseDto();
                    break;
                case "TextDto":
                    EntryDto = new TextDto();
                    break;
            }
        }

        internal void HandleSubmit()
        {
            // Handle form submission
        }
    }

}
