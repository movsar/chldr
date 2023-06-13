using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class TranslationEditViewModel : EditFormViewModelBase<TranslationDto, TranslationValidator>
    {
        #region Fields and Properties
        [Parameter]
        public TranslationDto Translation { get; set; } = new TranslationDto();
        #endregion

        [Parameter]
        public Action<string> OnDelete { get; set; }

        public void Delete()
        {
            OnDelete?.Invoke(Translation.TranslationId);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Translation == null)
            {
                return;
            }

            // Init code
        }
    }
}
