using domain.DatabaseObjects.Dtos;
using domain.Validators;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class TranslationEditViewModel : EditFormViewModelBase<TranslationDto, TranslationValidator>
    {
        #region Fields and Properties
        [Parameter]
        public TranslationDto TranslationDto { get; set; } = new TranslationDto();
        #endregion

        [Parameter] public Action<string> Remove { get; set; }
        [Parameter] public Action<TranslationDto> Promote { get; set; }
        public bool CanEditTranslation { get; private set; } = true;
        public bool CanRemoveTranslation { get; private set; }

        public void EditHandler()
        {
            CanEditTranslation = UserStore.CurrentUser?.CanEdit(TranslationDto.Rate, TranslationDto.UserId!) == true;
        }

        public void PromoteHandler()
        {
            Promote?.Invoke(TranslationDto);
        }
        public void RemoveHandler()
        {
            Remove?.Invoke(TranslationDto.TranslationId);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (TranslationDto.CreatedAt != DateTimeOffset.MinValue)
            {
                CanEditTranslation = false;
                CanRemoveTranslation = UserStore.CurrentUser?.CanRemove(TranslationDto.Rate, TranslationDto.UserId, TranslationDto.CreatedAt) == true;
            }
            await base.OnParametersSetAsync();
        }
    }
}
