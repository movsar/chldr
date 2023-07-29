using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Validators;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class TranslationEditViewModel : EditFormViewModelBase<TranslationDto, TranslationValidator>
    {
        #region Fields and Properties
        [Parameter]
        public TranslationDto TranslationDto { get; set; } = new TranslationDto();
        #endregion

        [Parameter]
        public Action<string> OnDelete { get; set; }
        public bool CanEditTranslation { get; private set; } = false;
        public bool CanRemoveTranslation { get; private set; }

        public void EditHandler()
        {
            CanEditTranslation = UserStore.CurrentUser?.CanEdit(TranslationDto.Rate, TranslationDto.UserId!) == true;
        }
        public void RemoveHandler()
        {
            OnDelete?.Invoke(TranslationDto.TranslationId);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (TranslationDto.CreatedAt == DateTimeOffset.MinValue)
            {
                CanEditTranslation = true;
            }
            else
            {
                CanRemoveTranslation = UserStore.CurrentUser?.CanRemove(TranslationDto.Rate, TranslationDto.UserId, TranslationDto.CreatedAt) == true;
            }
            await base.OnParametersSetAsync();
        }
    }
}
